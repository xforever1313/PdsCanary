@Library( "X13JenkinsLib" )_

def CallDevops( String arguments )
{
    dir( "checkout" )
    {
        X13Cmd( "dotnet run --project='./DevOps/DevOps/DevOps.csproj' -- ${arguments}" );
    }
}

def Build()
{
    CallDevops( "--target=build" );
}

def RunTests()
{
    CallDevops( "--target=run_tests" );
}

def Publish()
{
    CallDevops( "--target=publish" );
}

pipeline
{
    agent
    {
        label "ubuntu && docker";
    }
    environment
    {
        DOTNET_CLI_TELEMETRY_OPTOUT = 'true'
        DOTNET_NOLOGO = 'true'
    }
    options
    {
        skipDefaultCheckout( true );
    }
    stages
    {
        stage( "Clean" )
        {
            steps
            {
                cleanWs();
            }
        }
        stage( "checkout" )
        {
            steps
            {
                checkout scm;
            }
        }
        stage( "In Dotnet Docker" )
        {
            agent
            {
                docker
                {
                    image 'mcr.microsoft.com/dotnet/sdk:8.0'
                    args "-e HOME='${env.WORKSPACE}'"
                    reuseNode true
                }
            }
            stages
            {
                stage( "Build" )
                {
                    steps
                    {
                        Build();
                    }
                }
                stage( "Run Tests" )
                {
                    steps
                    {
                        RunTests();
                    }
                    post
                    {
                        always
                        {
                            X13ParseMsTestResults(
                                filePattern: "checkout/TestResults/PdsCanary.Tests/*.xml",
                                abortOnFail: true
                            );
                        }
                    }
                }
                stage( "Publish" )
                {
                    steps
                    {
                        Publish();
                    }
                }
                stage( "Archive" )
                {
                    steps
                    {
                        archiveArtifacts "checkout/dist/bluesky/zip/*.*";
                    }
                }
            }
        }
    }
}

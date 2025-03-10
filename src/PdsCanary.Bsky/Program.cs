//
// PdsCanary - A bot chirps every so often on the PDS.
// Copyright (C) 2024 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using PdsCanary.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Prometheus;
using Serilog;

namespace PdsCanary.Bsky
{
    public class Program
    {
        public static int Main( string[] args )
        {
            Console.WriteLine( $"Version: {typeof( PdsCanaryConfig ).Assembly.GetName()?.Version?.ToString( 3 ) ?? string.Empty}." );

            var config = new PdsCanaryConfig();
            if( config.TryValidate( out string error ) == false )
            {
                Console.WriteLine( "Bot is misconfigured" );
                Console.WriteLine( error );
                return 1;
            }

            Serilog.ILogger? log = null;

            void OnTelegramFailure( Exception e )
            {
                log?.Warning( $"Telegram message did not send:{Environment.NewLine}{e}" );
            }

            using var httpClient = new BskyHttpClientFactory( config );
            try
            {
                log = HostingExtensions.CreateLog( config, OnTelegramFailure );

                WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

                builder.Logging.ClearProviders();
                builder.Services.AddControllersWithViews();
                builder.Host.UseSerilog( log );
                builder.Services.AddSingleton<IHttpClientFactory>( httpClient );
                builder.Services.ConfigurePdsServices<SkeetJob, PdsCanaryConfig>( config );
                builder.WebHost.UseUrls( $"http://0.0.0.0:{config.Port}" );

                WebApplication app = builder.Build();
                app.UseRouting();

                // Per https://learn.microsoft.com/en-us/aspnet/core/diagnostics/asp0014?view=aspnetcore-8.0:
                // Warnings from this rule can be suppressed if
                // the target UseEndpoints invocation is invoked without
                // any mappings as a strategy to organize middleware ordering.
                #pragma warning disable ASP0014 // Suggest using top level route registrations
                app.UseEndpoints(
                    endpoints =>
                    {
                        endpoints.MapMetrics( "/Metrics" );
                    }
                );
                #pragma warning restore ASP0014 // Suggest using top level route registrations

                log.Information( "Application Running..." );

                app.Run();
            }
            catch( Exception e )
            {
                if( log is null )
                {
                    Console.Error.WriteLine( "FATAL ERROR:" );
                    Console.Error.WriteLine( e.ToString() );
                }
                else
                {
                    log.Fatal( "FATAL ERROR:" + Environment.NewLine + e );
                }
                return 2;
            }
            finally
            {
                log?.Information( "Application Exiting" );
            }

            return 0;

        }
    }
}
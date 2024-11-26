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

using System.Diagnostics;
using Quartz;
using Serilog;

namespace PdsCanary.Common
{
    public abstract class BaseMessageJob : IJob
    {
        // ---------------- Fields ----------------

        private readonly ILogger log;

        // ---------------- Constructor ----------------

        protected BaseMessageJob( ILogger log )
        {
            this.log = log;
        }

        // ---------------- Functions ----------------

        public async Task Execute( IJobExecutionContext context )
        {
            try
            {
                DateTime timeStamp = context.FireTimeUtc.DateTime;

                log.Information( "Sending Message..." );
                await SendMessage( timeStamp, context.CancellationToken );
                log.Information( "Sending Message...Done!" );
            }
            catch( Exception e )
            {
                this.log.Error( $"Error sending message: {Environment.NewLine}{e}" );
            }
        }

        protected abstract Task SendMessage( DateTime utcTime, CancellationToken cancelToken );

        public static string GetMessageString( DateTime time, TimeSpan uptime )
        {
            return
                $"Chirp! The PDS at at.shendrick.net is still online as of: {time.ToString( "dddd, MMMM d yyyy, h:00tt" )} server time.{Environment.NewLine}" +
                $"Server's been up for {uptime.Days} days, {uptime.Hours} hours. #Uptime";
        }
    }
}

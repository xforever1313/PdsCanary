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

namespace PdsCanary.Tests.Common
{
    [TestClass]
    public class MessageCheckerTests
    {
        // ---------------- Tests ----------------

        /// <summary>
        /// Ensures 12 bongs and the longest month (September)
        /// and longest day (Wednesday)
        /// can fit in 160 characters.
        /// </summary>
        [TestMethod]
        public void CheckAt12AM()
        {
            // Setup
            var time = new DateTime( 2022, 9, 21, 0, 0, 0, DateTimeKind.Local );
            const string timeStamp = "Wednesday, September 21 2022, 12:00AM";
            var uptime = new TimeSpan( 100, 23, 0, 0, 0 );

            // Act / Check
            DoGetMessageStringTest( time, uptime, timeStamp, 100, 23 );
        }

        [TestMethod]
        public void CheckAt4PM()
        {
            // Setup
            var time = new DateTime( 2022, 10, 31, 16, 0, 0, DateTimeKind.Local );
            const string timeStamp = "Monday, October 31 2022, 4:00PM";
            var uptime = new TimeSpan( 3, 16, 23, 5 ); ;

            // Act / Check
            DoGetMessageStringTest( time, uptime, timeStamp, 3, 16 );
        }

        [TestMethod]
        public void CheckAt3PM()
        {
            // Setup
            var time = new DateTime( 2022, 3, 13, 15, 0, 0, DateTimeKind.Local );
            const string timeStamp = "Sunday, March 13 2022, 3:00PM";
            var uptime = TimeSpan.Zero;

            // Act / Check
            DoGetMessageStringTest( time, uptime, timeStamp, 0, 0 );
        }

        [TestMethod]
        public void CheckAt2PM()
        {
            // Setup
            var time = new DateTime( 2022, 11, 20, 14, 0, 0, DateTimeKind.Local );
            var uptime = new TimeSpan( 1, 1, 29, 0 );
            const string timeStamp = "Sunday, November 20 2022, 2:00PM";

            // Act / Check
            DoGetMessageStringTest( time, uptime, timeStamp, 1, 1 );
        }

        [TestMethod]
        public void CheckAt1PM()
        {
            // Setup
            var time = new DateTime( 2022, 6, 19, 13, 0, 0, DateTimeKind.Local );
            var uptime = new TimeSpan( 1, 0, 0, 0 );
            const string timeStamp = "Sunday, June 19 2022, 1:00PM";

            // Act / Check
            DoGetMessageStringTest( time, uptime, timeStamp, 1, 0 );
        }

        // ---------------- Test Helpers ----------------

        private void DoGetMessageStringTest(
            DateTime expectedTime,
            TimeSpan expectdUptime,
            string timestampPortion,
            int dayPortion,
            int hourPortion
        )
        {
            string expectedMessage = $"Chirp! The PDS at at.shendrick.net is still online as of: {timestampPortion} server time.{Environment.NewLine}" +
                $"Server's been up for {dayPortion} days, {hourPortion} hours. #Uptime";

            string actualMessage = BaseMessageJob.GetMessageString( expectedTime, expectdUptime );
            Assert.AreEqual( expectedMessage, actualMessage );

            // Messages must be less than 300 characters (Blue Sky's limit).
            Assert.IsTrue( actualMessage.Length <= 300 );
        }
    }
}
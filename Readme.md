# PDS Canary

This is a Blue Sky bot that attempts to post to a given Blue Sky PDS.
If the message appears in the Blue Sky UI, then the PDS is online and accepting posts.

## Configuration

Set the following as Environment Variables

* **BSKY_USER** - The handle to login as.
* **BSKY_PASSWORD** - The app password to login as (regular password can be used, but app passwords are recommended).
* **CRON_STRING** - How often to send the "chirp" message in the form of a [cron string](https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#cron-expressions).
* **PDS_URL** - The PDS to post to.  Defaulted to https://bsky.social
* **LOG_FILE** - Where to output messages to.  Do not specify to not log messages.
* **TELEGRAM_BOT_TOKEN** - Optional, but allows warning and error messages to be sent to Telegram.  This and TELEGRAM_CHAT_ID must both be specified.
* **TELEGRAM_CHAT_ID** - Optional, but allows warning and error messages to be sent to Telegram.  This and TELEGRAM_BOT_TOKEN must both be specified.

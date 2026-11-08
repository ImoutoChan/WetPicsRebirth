# Deployment

GitHub Actions runs on a pushed tag or a manual workflow dispatch. It builds the
Docker image, publishes it to Docker Hub, and connects to the server over SSH to
execute `/root/update.wetpicsrebirthbot.sh`.

Required Actions secrets:

| Secret | Purpose |
| --- | --- |
| `DOCKER_HUB_USERNAME` | Docker Hub account publishing the image |
| `DOCKER_HUB_ACCESS_TOKEN` | Token with permission to push this repository |
| `HOST` | Deployment server address |
| `USERNAME` | SSH deployment user |
| `KEY` | Dedicated deployment private key |
| `HOST_FINGERPRINT` | SHA256 fingerprint of the server's SSH host key |

Keep .NET SDK 3.1 in the workflow for GitVersion. The application itself is built
inside Docker using its existing .NET 9 SDK image.

The server directory `/home/WetPicsRebirth` contains one `docker-compose.yml` and
a private `.env` file. Copy `Deploy/docker-compose.yml` to that directory. The
application receives its settings through the Compose service's `env_file`;
Azure App Configuration and production JSON settings are not used.

Use .NET configuration names in `.env`, replacing `:` with `__` for nested keys.
For example, fill these entries with the existing deployment's actual values:

```dotenv
POSTGRES_PASSWORD='existing database password'
TelegramApiKey='new bot token'
WebHookAddress='https://sis.pink/api/wetpicsrebirth/update'
WhiteList__0='telegram user ID'
ModeratorId='telegram moderator ID'
ChannelInviteLink='channel invite link'
AccessLink='access information link'
Danbooru__Username='existing username'
Danbooru__ApiKey='existing API key'
Danbooru__Delay=1
Danbooru__BotUserAgent='WetPicsRebirth/v1'
```

Keep `.env` readable only by the deployment user (`chmod 600`), outside Git and
the Docker build context. `POSTGRES_PASSWORD` must match the existing database:
changing this setting does not change the password of an initialized database.
Compose builds the bot's database connection string from this value.

Nginx forwards `https://sis.pink/api/wetpicsrebirth/update` to
`127.0.0.1:42426`. The PostgreSQL data remains in the existing `pgdata` volume.

Install `WetPicsRebirth.Update.sh` as `/root/update.wetpicsrebirthbot.sh`, with
executable permissions. The script uses `docker compose` to stop the stack,
pull the bot image, and start the stack. It also stops and starts PostgreSQL.

The deployment SSH key can be limited to this script using an `authorized_keys`
entry beginning with `restrict,command="/root/update.wetpicsrebirthbot.sh"`.

After rotating the Telegram token in BotFather, replace `TelegramApiKey` in
`.env` and recreate the bot container. Autoposting is controlled separately by
the `/wetpicson` and `/wetpicsoff` bot commands and is currently left disabled.

# Nano Ping-Pong
The nano ping-pong service provides a listener that will receive and refund any transactions sent to the address it owns as well as a simple web app to display the ping-pong address.  These components can be used together or separately.

## Environment Variables
The container can be configured using the following environment variables:

| Variable           | Required      | Default                       | Description                                                                                                                                                                                                     |
| :---               | :---          | :---                          | :---                                                                                                                                                                                                            |
| Node               | Yes           |                               | The URL to use to access the node that will process requests (e.g., http://node:7076)                                                                                                                           |
| WorkServer         | Yes           |                               | The URL to use to access the node or work server that will generate work (e.g., http://work-server:7076)                                                                                                        |
| Context            | Yes           | nano                          | nano or banano                                                                                                                                                                                                  |
| SeedFile           | Yes           | /run/secrets/nano-ping.seed   | The location of the file on disk containing the nano or banano seed                                                                                                                                             |
| TickSeconds        | Yes           | 1                             | How often to check for pending receivable blocks                                                                                                                                                                |
| Cache              | Yes           | true                          | true to try to cache nano work ahead of time; false otherwise.  Only set to true if you've funded the nano account with 1 XNO to allow for returning funds before receiving them.  Ignored when Context=banano. |
| DefaultRaw         | Yes           | 10000000000000000000000000000 | Raw amount to pre-fill in Natrium/Kalium via the QR code; defaults to 0.01 XNO or 0.1 BA                                                                                                                        |
| ASPNETCORE_URLS    | Yes           | http://+:2022                 | The URL on which to listen for incoming requests; change this if you want to change the port on which the web app is listening                                                                                  |
| DonationAddress    | No            |                               | The address where people can send donations as a thank you for running the service; not required                                                                                                                |

**Node and WorkServer are required for every container running the listener.**

## Seed File

## Docker Compose Examples

### Bundled

### Separated

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
```
{
  "seed": "YOUR_SEED_HERE"
}
```

## Docker Compose Examples
These examples assume you are using traefik as a reverse proxy, because it's dope.  But you do you.
They also assume you are running on Docker Swarm Mode.

### Bundled
```
version: '3.8'

services:

  node:
    image: 'bananocoin/banano:V##'
    networks:
      - banano
    ... clipped for brevity ...
    
  monitor:
    image: 'nanotools/nanonodemonitor:v##'
    networks:
      - traefik
      - banano
    ... clipped for brevity ...
    
  ping:
    image: 'cinderblockgames/nano-ping-pong:1.0.0'
    secrets:
      - banano-ping.seed
    volumes:
      - '/run/homelab/banano/ping-logs:/run/logs'
    environment:
      - 'Context=banano'
      - 'SeedFile=/run/secrets/banano-ping.seed'
      - 'Node=http://node:7072'
      - 'WorkServer=http://node:7072'
      - 'DonationAddress=ban_3s9c389jsom8gqsp8zbeampfi7kpipdnzmp1rkbnzstemursdsopsz3h8mg1'
    networks:
      - traefik
      - banano
    deploy:
      mode: replicated
      replicas: 1
      labels:
        - 'traefik.enable=true'
        - 'traefik.docker.network=traefik'
        - 'traefik.http.routers.banano-ping.rule=Host(`ping.banano.kga.earth`)'
        - 'traefik.http.routers.banano-ping.entrypoints=web-secure'
        - 'traefik.http.routers.banano-ping.tls'
        - 'traefik.http.services.banano-ping.loadbalancer.server.port=2022'

networks:
  traefik:
    external: true
  banano:
    external: true

secrets:
  banano-ping.seed:
    external: true
```

### Separated
```
version: '3.8'

services:

  node:
    image: 'bananocoin/banano:V##'
    networks:
      - banano
    ... clipped for brevity ...
    
  monitor:
    image: 'nanotools/nanonodemonitor:v##'
    networks:
      - traefik
      - banano
    ... clipped for brevity ...
    
  ping-listener:
    image: 'cinderblockgames/nano-ping-pong:1.0.0'
    command: dotnet /app/listener/NanoPingPong.dll
    secrets:
      - banano-ping.seed
    volumes:
      - '/run/homelab/banano/ping-logs:/run/logs'
    environment:
      - 'Context=banano'
      - 'SeedFile=/run/secrets/banano-ping.seed'
      - 'Node=http://node:7072'
      - 'WorkServer=http://node:7072'
    networks:
      - banano
    deploy:
      mode: replicated
      replicas: 1
    
  ping-web:
    image: 'cinderblockgames/nano-ping-pong:1.0.0'
    command: dotnet /app/web/NanoPingPong.Web.dll
    secrets:
      - banano-ping.seed
    environment:
      - 'Context=banano'
      - 'SeedFile=/run/secrets/banano-ping.seed'
      - 'DonationAddress=ban_3s9c389jsom8gqsp8zbeampfi7kpipdnzmp1rkbnzstemursdsopsz3h8mg1'
    networks:
      - traefik
      - banano
    deploy:
      mode: replicated
      replicas: 2
      labels:
        - 'traefik.enable=true'
        - 'traefik.docker.network=traefik'
        - 'traefik.http.routers.banano-ping.rule=Host(`ping.banano.kga.earth`)'
        - 'traefik.http.routers.banano-ping.entrypoints=web-secure'
        - 'traefik.http.routers.banano-ping.tls'
        - 'traefik.http.services.banano-ping.loadbalancer.server.port=2022'

networks:
  traefik:
    external: true
  banano:
    external: true

secrets:
  banano-ping.seed:
    external: true
```

# rabbitmq-dot-net

Esse projeto foi desenvolvido em .net 8.0

As principais tecnologias e conceitos utilizados foram:

 - .net 8.0
 - Rabbitmq
 - MassTransit
 - CronJob
 - Quartz
 - Consumer

## Rodar Projeto

### `appsettings.json`

- atualizar a `MassTransitConfigs` com `host, User, password` do seu message broker

- Defina quando o job irá rodar em `JobConfigs:CronExpression`,
para ajudar na cronExpression clique [aqui](http://www.cronmaker.com/)

## Rotas

### `Bus/EnviarMensagem`

Envia através da queryParam `TotalMessagem` a quantidade de mensagens enviadas

### `Job/Run`

Chama o job `SendMessageJob` para eviar uma pensagem para a fila

## `Obs`

- Acompanhe pelos logs os envios das mengens e os consumos
- o Job esta configurado para rodar de 1 em 1 minuto conforme a cronExpression
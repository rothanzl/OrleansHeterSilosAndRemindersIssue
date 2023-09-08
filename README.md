# Orleans Performance Tests in Azure Container Apps

This project originates from fork of https://github.com/Azure-Samples/Orleans-Cluster-on-Azure-Container-Apps


## Tests

Each test is assigned by number and has own branch

1. `test/01-max-grains` - Max limit grains in silo tests
2. `test/02-stream-broadcast` - Orleans broadcast channels tests

### Max limit grains in silo tests

Každé silo má spuštěou servisu, který vytváří v paralelních smyškách grainy.
Je zvolena PreferLocalPlacement strategy.
Hlavní dávka testů má state o velikosti 21KB, který přibližně odpovídá současnému LockerGrain state.
Každé silo má implementované rest api, které zasílá informace o clusteru a to kolik je aktivních sil v clusteru a kolik je aktivních testovacích grainů.

Tester zapne na clusteru vytváření nových grainů a začne ve smyčce se dotazovat na inforamce o cluster.
Test probíhá dokud je REST API odpovídá. Test končí při prvním ne success odpovědi na REST API.
To bývá při
- přeplnění paměti nad 3.4 GB
- ~ 150,000 aktivních grainů při velikosti stavu 21 KB (3.15 GB zabírá stav celkem)
- ~ 868,000 aktivních grainů bez stavu



Azure container apps mi nedovolilo vytvořit více než 38 replik. A nepodporovalo více než 48 replik současně běžících. Při spuštění rolling update se zasekl právě na 48 replikách.


Nepodařilo se mi vyinstancovat 60 milionů grainů.
Při zapnutí CosmosDB byli výsledky stejné, jen se musí správně naškálovat RUs aby test probíhal dostatečně rychle.
Local placement strategy je krytická. Bez ní je test při výce silech (10 a více) mnohem delší (na 10 silech je test cca 12 krát delší)
Testy trvaly cca 10 minut, tedy aktivovat 150,000 grainů trvá při optimálním vytížení 10 minut.


### Orleans broadcast channels tests

https://learn.microsoft.com/en-us/dotnet/orleans/streaming/broadcast-channel

#### 1. Test

1. Start cluster with 1 silo
2. Tester enable production to broadcast channel
3. The producer start producing in one thread in loop 1KB message containing increasing counter.
4. The consumer checking consistency of increasing counter
5. Test in loop check state of consumer
6. Test ended in 10 minutes

#### 2. Test

Produce faster than consume, measure when it failed.


## Deploy

Deploy pipeline is triggered by push to `deploy` branch.

## Solution structure

Projects:
1. Silo
2. Tester
3. Dashboard
4. Abstraction

### Silo

System under test.

### Tester

Tester.
Implements NBomber.
Handling by REST API.

### Dashboard

Orleans Dashboard.

### Abstraction

Shared library.

## Common trouble and troubleshooting

### Cannot deploy due to credentials

This error happened almost any time when had been created new resources in Azure.


    ERROR: Failed to retrieve credentials for container registry. Please provide the registry username and passwor
    Error: Error: az cli script failed.



The solution is to retry `deploy` job until pass. (retry about 5 times)
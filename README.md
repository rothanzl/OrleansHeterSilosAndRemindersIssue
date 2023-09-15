# Orleans Performance Tests in Azure Container Apps

This project originates from fork of https://github.com/Azure-Samples/Orleans-Cluster-on-Azure-Container-Apps

Login to Azure is provided by `Managed Identity`.


## Tests

Each test is assigned by number and has own branch

1. `test/01-max-grains` - Max limit grains in silo tests
2. `test/02-stream-broadcast` - Orleans broadcast channels tests
3. `test/03-reminders` - Orleans reminders limit tests

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

##### Results

1. Pořadí doručení zpráv není zaručeno. Pokud je již grain aktivován v době produkování, je pořadí vždy v pořádku. 
Pokud nění konzumer v době publikování aktivován a jdou dvě zprávy přímo po sobě, často je jejich pořadí obráceně.
2. Konzumace zpráv je jednovláknová a čeká se na dokončení konzumace zprávy před startem další metody 
(konzumace zprávy z broadcast kanálu nebo požadavek na rozhraní grainu).
3. Požadavky na rozhranní grainu jsou řazeny za již čakající zprávy. Pokud je konzumer zahlcen nezpracovanými zprávami, 
požadavky jsou pozdržen dokud se fronta nezpracuje. 
4. Test pozastavení konzumace a přetečení fronty. Fronta přetekla při velikosti 96512 o velikosti zprávy 21KB.
Při přetečení fronty se producer nedozví, že se zpráva nepovedla doručit.


#### 2. Test

Produce faster than consume, measure when it failed.

##### Result:

Producer doesn't know, that channel throws messages.
Only records in logs inform about overloading.

The channel can store up to 20,000 undelivered messages. 
Often much less only about 15,000.


### Orleans reminders limit tests

TBD


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
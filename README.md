# Issue heterogeneous silos and reminders 

The cluster consists of 2 silos. 
Grain `RemindGrain` has interface and implementation only in one silo. 
When the grain `RemindGrain` invoke `RegisterOrUpdateReminder` method it often throws exception.

Exception:

    Orleans.Runtime.OrleansMessageRejectionException: SystemTarget sys.svc.user.36F5F3BF/192.168.1.106:11112@53693720 not active on this silo. Msg=Request [S192.168.1.106:11111:53693925 remind/1921681106-4]->[S192.168.1.106:11112:53693720 sys.svc.user.36F5F3BF/192.168.1.106:11112@53693720] Orleans.IReminderService.RegisterOrUpdateReminder(remind/1921681106-4, shipmentState, 00:05:00, 00:05:00) #64
        at Orleans.Serialization.Invocation.ResponseCompletionSource1.GetResult(Int16 token) in /_/src/Orleans.Serialization/Invocation/ResponseCompletionSource.cs:line 230
        at System.Threading.Tasks.ValueTask1.ValueTaskSourceAsTask.<>c.<.cctor>b__4_0(Object state)
    --- End of stack trace from previous location ---


## Setup

To reproduce the issue, set `CosmosDbKey` `CosmosDbUri` environment variables.
Run `Silo` and `Dashboard` projects.
The issue start automatically after silo `Silo` started.
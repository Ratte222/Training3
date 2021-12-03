# Useful links
- How to: Use Named Pipes for Network Interprocess Communication https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-use-named-pipes-for-network-interprocess-communication?redirectedfrom=MSDN
- async tutuorials
    - https://www.youtube.com/watch?v=lHuyl_WTpME
    - https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-simple-parallel-foreach-loop
    - https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-parallel-for-loop-with-thread-local-variables
    - https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-handle-exceptions-in-parallel-loops
    - https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/exception-handling-task-parallel-library
- Mongo transaction 
    - https://www.mongodb.com/developer/how-to/transactions-c-dotnet/
    
# How use
Start Training3 and NotificationService.
Use endpoint https://localhost:5001/api/TestNotification POST to create notification.
Example body for request:
```json
{
  "typeNotification": 0,//send from email
  "recipient": "Taya@gmail.com",
  "sender": "Artur",
  "header": "Test",
  "messageBody": "Test body"
}
```
Messages will be sent from the account in the training3 project appsettings.
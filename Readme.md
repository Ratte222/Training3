# What was done
- work with mapster library
- work with server-sent events. To be in SSEController. Has clsint to be in "TestServer-Sent_Events2.html"
- added compressing ot response to middleware (https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-5.0)

## Example of work server-sent events
Console in browser at the file "TestServer-Sent_Events2.html" after sending a message to endpoint "AddExpense".
![][image_ref_1]


[image_ref_1]:
data:image/png;base64,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


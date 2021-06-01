# Arheisel.REST
Basic REST Async library with GET and POST functionality

## Reference

`async Task<string> PostAsync(string uri, string json)` Generates a POST Request with the given json. Returns server response. Throws `RestException` on Error

`async Task<string> PostAsync(string uri, HttpContent data)` Generates a POST Request with the given HttpContent. Returns server response. Throws `RestException` on Error

`async Task<string> GetAsync(string uri)` Generates a GET Request. Returns server response. Throws `RestException` on Error

`static string JsonFromTemplate(string name, Dictionary<string, string> args)` Loads a text file from Disk and replaces the keywords marked as {{keyword}} with the corresponding value in the key-value pair for every entry in the`args` Dictionary.




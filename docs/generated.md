# Rhythm.Net.Core

<table>
<tbody>
<tr>
<td><a href="#networkhelper">NetworkHelper</a></td>
<td><a href="#senddataresult">SendDataResult</a></td>
</tr>
</tbody>
</table>


## NetworkHelper

Assists with operations related to networking.

### ConstructQueryString(uri, data)

Constructs a query string from the specified URL and data.

| Name | Description |
| ---- | ----------- |
| uri | *System.Uri*<br>The URL (potentially containing a query string). |
| data | *System.Collections.Generic.IDictionary{System.String,System.String}*<br>The data. |

#### Returns

The query string.

### GetResponse(url)

Returns the response string downloaded from the request to the specified URL.

| Name | Description |
| ---- | ----------- |
| url | *System.String*<br>The URL to make the request to. |

#### Returns

The response string.

### SendData(url, data, method, sendInBody)

Sends a web request with the data either in the query string or in the body.

| Name | Description |
| ---- | ----------- |
| url | *System.String*<br>The URL to send the request to. |
| data | *System.Collections.Generic.IDictionary{System.String,System.String}*<br>The data to send. |
| method | *System.String*<br>The HTTP method (e.g., GET, POST) to use when sending the request. |
| sendInBody | *System.Boolean*<br>Send the data as part of the body (or in the query string)? |

#### Returns

An object containing details about the result of the attempt to send the data.

#### Remarks

Parts of this function are from: http://stackoverflow.com/a/9772003/2052963 and http://stackoverflow.com/questions/14702902


## SendDataResult

Stores the result of an attempt to send data over a network.

### HttpWebResponse

The HTTP web response.

### ResponseError

The response error, if one occurs.

### ResponseText

The response text.

### Success

Was the request a success?

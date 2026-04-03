# Altec Api

## Introduction
This is an API built for the company Altec to make our job easier.

## Requirements
- .NET 9
- IDE

## Installation
1. Clone the repository
2. open the project
3. run the project 
4. open http://localhost:5258

## Configuration
Update `appsettings.json` with the NiceLabel SDK API URL:
```json
{
  "NiceLabelApi": {
    "BaseUrl": "https://localhost:44368/"
  }
}
```

## Endpoints
All API endpoints.

### TSPL

#### Preview
Returns an label preview of format png.

`POST /api/tspl/preview`

**Request**

`json`

| Parameter  | Type | Required | Description |
|------------|------|----------|-------------|
| tspl | string | yes | The label TSPL |
| showBlockOutline | boolean | no | Show to outlining of block commands |
| images | dictionary | no | Images on the label |

**Response**

```json

```
- `200 OK` - Returns the label preview
- `400 Bad Request` - If parameters fails

### Printers

#### Discover
Returns a list of printers on the network

`GET /api/printers/discover`

**Request**

`query parameters`

| Parameter  | Type | Required | Description |
|------------|------|----------|-------------|
| subnets | string | no | The subnets to search on |

**Response**

```json
{
	"printers": [
		{
			"dnsName": "PRN-Ken.impuls-benelux.local",
			"ipAddress": "192.168.0.62",
			"model": "ATP-300 Pro",
			"port": 9100
		},
		{
			"dnsName": "PRN_Mikael.impuls-benelux.local",
			"ipAddress": "192.168.0.69",
			"model": "ATP-300Pro",
			"port": 9100
		},
    ]
}


```
- `200 OK` - Returns the list of printers
- `400 Bad Request` - If parameters fails

### NiceLabel

#### Variables
Returns a list of variables from a NiceLabel label file.

`POST /api/nicelabel/variables`

**Request**

`multipart/form-data`

| Parameter  | Type | Required | Description |
|------------|------|----------|-------------|
| label | file | yes | The NiceLabel label file |

**Response**

```json
[
  "Teller",
  "Name"
]
```
- `200 OK` - Returns list of strings
- `400 Bad Request` - If parameters fails

#### Print
Prints a NiceLabel label file.

`POST /api/nicelabel/print`

**Request**

`multipart/form-data`

| Parameter   | Type | Required | Description                                                                 |
|-------------|------|----------|-----------------------------------------------------------------------------|
| label       | file | yes | The NiceLabel label file                                                    |
| quantity    | int | yes | The print amount                                                            |
| printerName | string | no | The name of the printer to print to if no printer is specified in the label |

**Response**
- `200 OK` - "Printing label..."
- `400 Bad Request` - If validation fails
- `500 Internal Server Error` - If printing fails

## License
This is an internal tool developed for Altec. All rights reserved.

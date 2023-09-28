# The Protocol

### MessageType.Handshake -> ToApp
The basic handshake, to let the App know the plogon is ready.
| Var Type  	| Parameter 				| Note		 		|
| ------------- | ------------- 			| ------------- 	|
| MessageType   | MessageType.Handshake  	|  the MessageType  |
| int		    | 0  						|  always 0			|
| string	    | (int)PId  				|  the process Id 	|

            msgType    = MessageType.Handshake,
            msgChannel = 0,
            message    = ProcessId
---

### MessageType.Version -> ToApp
Sends the plogon version. The string looks like this: "Pid:1.2.3.4"
| Var Type  	| Parameter 				| Note		 		|
| ------------- | ------------- 			| ------------- 	|
| MessageType   | MessageType.Version	  	|  the MessageType  |
| int		    | 0  						|  always 0			|
| string	    | (int)PId:AssemblyVersion	|  "Pid:Version" 	|

		msgType = MessageType.Version,
		msgChannel = 0,
		message = (int)ProcessId + ":" + (string)Version
---

### MessageType.SetGfx -> ToApp
Indicates if the gfx settings are low or high
| Var Type  	| Parameter 				| Note		 		|
| ------------- | ------------- 			| ------------- 	|
| MessageType   | MessageType.SetGfx	  	|  the MessageType  |
| int		    | 0  						|  always 0			|
| string	    | (int)PId:bool				|  "Pid:1" 			|

		msgType    = MessageType.SetGfx,
		msgChannel = 0,
		message    = (int)ProcessId + ":" + (bool)IsGFXLow
---

### MessageType.SetGfx <- From App
Sets the client gfx to low or high
| Var Type  	| Parameter 				| Note		 		|
| ------------- | ------------- 			| ------------- 	|
| MessageType   | MessageType.SetGfx	  	|  the MessageType  |
| int		    | 0  						|  always 0			|
| string	    | (int)PId:bool				|  1=low; 0=high	|

		msgType    = MessageType.SetGfx,
		msgChannel = 0,
		message    = (int)ProcessId + ":" + (bool)lowGFX
---

### MessageType.NameAndHomeWorld -> To App
Send the client Name and Homeworld
| Var Type  	| Parameter 				| Note		 		|
| ------------- | ------------- 			| ------------- 	|
| MessageType   | MessageType.SetGfx	  	|  					|
| string	    | (int)PId:string:string	|  					|

		msgType = MessageType.NameAndHomeWorld,
		message = (int)ProcessId + ":" + Name + ":" + HomeWorld
---


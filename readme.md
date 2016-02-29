Demo project using Orleans framework and Rx.NET


SiloHost/program.cs: entry point. Controller is created, then 5 devices are created and assigned to controller

Grains/Device: Device class. Used Rx generator to emulate sensor data. Default temperature is 50. Every 1-5 seconds current temperature is changed with dt=2 and sent to Rx observable collection. Every second average temperature in window of 3 seconds 
 is calculated. Every second timer emits average temperature to Orleans streams of assigned controllers
 
 Grains/Controller: Controller class listens to Orleans stream for device data and redirects data to local Rx observable, where it is filtered, aggregated, and processed by event listeners. Every recieved device message is logged to console. Every second average temperature across all devices is logged to console. If device data with temperature more than 55 recieved, it is logged to console
 
 Grains/ImmediateWindow: helper class with extension method for creating windowed observable, that starts outputting data immediately without waiting a full time window to start
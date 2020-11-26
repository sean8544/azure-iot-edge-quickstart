# azure-iot-edge-quickstart
This is some quickstart about how to use Azure iot Edge

Demo8： create a custom sensor module, this module will use moduleclient in C# sdk send telemetry data to cloud instead of using Microsoft SimulatedTemperatureSensor module.
You can follow step by step video：https://www.51azure.cloud/post/2020/11/12/azure-iot-edge8-debug-edge-module 

创建了一个新的Sensor module代替之前使用的微软提供的SimulatedTemperatureSensor，Sensor module是C#语言写的，使用moduleclient 客户端向外发送遥测消息。
可以参考完整的操作视频：https://www.51azure.cloud/post/2020/11/12/azure-iot-edge8-debug-edge-module

Demo9: Cerate a new Azure function module, function module get data from Custom sensor data then  save data to Azure sql Edge.
You can follow step by step video：https://www.51azure.cloud/post/2020/11/17/using-azure-function-on-edge-device-save-data-to-azure-sql-edge
创建了一个C#的Azure functions module，该module从Sensor 接受数据，然后写入到同一个物理Edge设备的Azure SQL Edge模块里。
可以参考完整的操作视频：https://www.51azure.cloud/post/2020/11/17/using-azure-function-on-edge-device-save-data-to-azure-sql-edge

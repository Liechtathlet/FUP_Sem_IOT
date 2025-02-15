﻿// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
#r "./libs/GrovePiTest.dll"


open GrovePiTest
open FSharp.Data
open System.IO
open System
open System.Threading

let blink = 
    printfn "Hello from F#"
    //GrovePiTest.Program.blink()
    0// return an integer exit code

// Timer functions
let startTimerAndCreateObservable timerInterval =
    // Setup timer
    let timer = new System.Timers.Timer(float timerInterval)

    // Autoreset and enable
    timer.AutoReset <- true
    timer.Enabled <- true

    // Return observable event
    timer.Elapsed  


// File functions
let writeData suffix (data:String) (date:DateTime) (location:String) = 
    let filepath = "./data/SensorData-" + suffix + ".csv"

    if not(File.Exists(filepath)) then
        printfn "Data-File doesn't exists, creating file"
        use streamWriter = new StreamWriter(filepath,false)
        streamWriter.WriteLine "Date;Time;SensorData;Location"
        streamWriter.Flush()
        streamWriter.Close()

    use streamWriter = new StreamWriter(filepath,true)
    [ date.ToString("dd.MM.yyyy");
      date.ToString("hh:mm:ss.fff");
      data;
      location]
    |> List.fold (fun r s -> r + s + ";") ""
    |> streamWriter.WriteLine

    streamWriter.Flush()
    streamWriter.Close()

// Location
let currentLocation = "Home@Brun"

// Read sensor values
let readTemperatureAndHumiditySensor() =
    5

let readLightSensor() =
    4

let readNoiseSensor() =
    10

// Event processors
let processTemperatureAndHumidityEvent() =
    let sensorValue = readTemperatureAndHumiditySensor()
    writeData "TH" (sensorValue.ToString()) DateTime.Now currentLocation

let processNoiseEvent() =
    let sensorValue = readNoiseSensor()
    writeData "N" (sensorValue.ToString()) DateTime.Now currentLocation

let processLightEvent() =
    let sensorValue = readLightSensor()
    writeData "L" (sensorValue.ToString()) DateTime.Now currentLocation


// Main Program

//Create TemperatureAndHumidity timer
let thEventStream = startTimerAndCreateObservable 100 

// Subscribe to event
thEventStream |> Observable.subscribe (fun _ -> processTemperatureAndHumidityEvent())

//Create Noise timer
let nEventStream = startTimerAndCreateObservable 100 

// Subscribe to event
nEventStream |> Observable.subscribe (fun _ -> processNoiseEvent())

//Create Light timer
let lEventStream = startTimerAndCreateObservable 100 

// Subscribe to event
lEventStream |> Observable.subscribe (fun _ -> processLightEvent())


//Wait for termination
Console.ReadLine()

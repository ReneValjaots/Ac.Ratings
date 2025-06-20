## Overview

**Ratings Manager** is a simple tool for rating cars in **Assetto Corsa**. You can use it with **Content Manager** or the standard (vanilla) version of the game. If you use Content Manager, you can also transfer your ratings back to it.

The app lets you quickly rate each car, mark extra features, and see key specs such as drivetrain and transmission type. The layout is inspired by car brand websites like Porsche and AMG.

> [!NOTE]
> **Compatibility**: Windows 10/11 | Assetto Corsa | Content Manager (optional)

## Key Features

- **Rate Cars** – Choose between **5-point** or **10-point** rating scales to evaluate cars based on criteria like handling, sound, and realism.
- **Mark Extra Features** – Track additional features like dashboard lights, animations, or extended physics.
- **Drivetrain & Transmission Info** – View drivetrain and transmission details extracted directly from each car’s original `.acd` file.
- **Car Filtering** – Quickly filter cars based on ratings, author, class, drivetrain and transmission type.
- **Customizable Power Units** – Customize how power figures are shown. Choose between kW, hp, PS, or CV in any combination (e.g. kW / hp or PS only).
- **Editable Engine Database** – The app uses a SQLite database to store engine specs (Displacement, Layout, Cylinders). You can edit and add new engine data for any car directly within the app.
- **Automatic Saving & Backups** – Changes are saved and backed up automatically when the app closes.
- **Modern & Customizable UI** – Choose from Light, Dark, or Black themes, and pick your preferred accent color.
- **Sync with Content Manager ratings** – Convert your ratings to a 5-point scale and transfer them back into Content Manager.

## Installation

1. Download the `Ratings Manager.zip` file from a release.
2. Extract the Ratings Manager folder to a location of your choice.
3. Run `Ratings Manager.exe` from the extracted folder.
> [!TIP]
> Right-click on `Ratings Manager.exe` and select Send to > Desktop (create shortcut) for quick access.

## First-Time Setup & Usage

1. On first launch, you will be asked to set your Assetto Corsa root folder. On Windows, it is typically located at:
   ```
   C:\Program Files (x86)\Steam\steamapps\common\assettocorsa
   ```
2. The app will restart to finalize the setup process.
3. Select a car from the list.
4. Rate it based on the given criteria.
5. Mark any extra features it includes.
6. The ratings and features will be saved automatically when application closes. 

## Optional: Kunos Engine Database

The release includes a pre-filled `engines.db` file with engine specs for all official Kunos cars. To use it:

1. Run the application at least once. This will create a `Data` folder inside your Ratings Manager folder.
2. Copy the `engines.db` file (located in the main Ratings Manager folder) into the newly created `Data` folder.
3. When prompted, choose to replace the existing empty file.
4. Restart the application. The engine information for all Kunos cars will now be displayed.

## Screenshots

![darkmode_home](https://github.com/user-attachments/assets/1e15a3fc-5e61-425c-ad3b-5931bc484de4)

![blackmode_home](https://github.com/user-attachments/assets/882a552b-dfbb-4b05-9147-a7008d3b45c4)

![lightmode_home](https://github.com/user-attachments/assets/da341bd1-b7fb-4ee1-ae1c-9ff3dd2e47af)

![darkmode_filter](https://github.com/user-attachments/assets/077f11b7-0915-47df-8728-c39c606b27ca)

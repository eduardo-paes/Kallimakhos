# Kallimakhos
*Clean Architecture Project Creator*

This is a C# program that creates a clean architecture project structure. It allows you to specify the project's name, location, and additional features like external services and UI types.

<p align="center">
  <img alt="License" src="https://img.shields.io/badge/license-MIT-blue.svg">
  <img alt="Build" src="https://img.shields.io/badge/build-passing-brightgreen.svg">
  <img alt="Version" src="https://img.shields.io/badge/version-0.0.1-orange.svg">
</p>

## Table of Contents

- [Clean Architecture Project Creator](#clean-architecture-project-creator)
  - [Table of Contents](#table-of-contents)
  - [Getting Started](#getting-started)
  - [Usage](#usage)
  - [License](#license)

## Getting Started

To get started with this program, follow the instructions below:

1. Make sure you have [.NET Core](https://dotnet.microsoft.com/download) installed on your system.

2. Clone this repository or download the C# code.

3. Open a command prompt or terminal.

4. Navigate to the directory where the C# code is located.

5. Run the program using the following command:

   ```bash
   dotnet run -np <PATH> <PROJECT_NAME> [-es] [-ui <UI_TYPE>]
   ```

   Replace `<PATH>` with the path where you want to create the project, `<PROJECT_NAME>` with the desired project name, `-es` to add external services (optional), and `-ui <UI_TYPE>` to add a UI project (optional). Valid UI types include grpc, webapi, webapp, mvc, console, angular, and react.

## Usage

Here are some examples of how to use the program:

- Create a clean architecture project without external services or UI:

  ```bash
  dotnet run -np /path/to/project MyProject
  ```

- Create a clean architecture project with external services:

  ```bash
  dotnet run -np /path/to/project MyProject -es
  ```

- Create a clean architecture project with a Web API UI:

  ```bash
  dotnet run -np /path/to/project MyProject -ui webapi
  ```

- Create a clean architecture project with a React UI and external services:

  ```bash
  dotnet run -np /path/to/project MyProject -es -ui react
  ```

## License

This project is licensed under the MIT License - see the [LICENSE](./LICENSE.md) file for details.

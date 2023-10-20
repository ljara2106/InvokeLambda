# Invoke Lambda

**Invoke Lambda** is a simple web application designed to remotely trigger AWS Lambda functions. It provides a user-friendly interface for selecting and invoking Lambda functions stored in your AWS account. This README provides an overview of the application's structure, its requirements, and how to set it up.

## Table of Contents
- [Purpose](#purpose)
- [Prerequisites](#prerequisites)
- [Application Structure](#application-structure)
- [Configuration](#configuration)
- [Logging](#logging)
- [Spam Prevention](#spam-prevention)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Purpose

The primary purpose of **Invoke Lambda** is to allow users to remotely invoke AWS Lambda functions without needing to access the AWS Management Console directly. It simplifies the process by providing a user-friendly interface with a dropdown menu to select and trigger Lambda functions.

## Prerequisites

Before using **Invoke Lambda**, ensure you have the following prerequisites in place:

- AWS credentials with the following:
    - `AWSAccessKeyId`: Your AWS Access Key ID.
    - `AWSSecretAccessKey`: Your AWS Secret Access Key.
    - `LambdaRegion`: The AWS region where your Lambda functions are hosted.
- AWS Lambda functions that you want to trigger.

## Application Structure

**Invoke Lambda** is divided into two main components:

### Frontend
- The user interface presents a dropdown selection where you can choose the Lambda function to invoke.
- It includes JavaScript functionality to disable the button during a cooldown period to prevent spam.

### Backend
- **web.config**: This configuration file contains AWS credentials required to access AWS resources.
- **lambdafunctions.xml**: This XML file contains function names and their corresponding alias names, which are visible in the dropdown menu.
- Logging functionality is implemented:
    - Errors are written to a log file.
    - User IP addresses and timestamps are logged whenever a function is invoked.

## Configuration

Before using the application, configure the following settings:

1. **web.config**:
    - `AWSAccessKeyId`: Your AWS Access Key ID.
    - `AWSSecretAccessKey`: Your AWS Secret Access Key.
    - `LambdaRegion`: The AWS region where your Lambda functions are hosted.

2. **lambdafunctions.xml**:
    - Populate this XML file with the Lambda function names and their alias names for the dropdown menu.

## Logging

**Invoke Lambda** provides a basic logging system:

- Error logging: Any errors that occur are logged in a separate file.
- Invocation logging: Whenever a user invokes a function, their IP address and a timestamp are logged.

## Spam Prevention

To prevent spamming of the "Invoke" button, a JavaScript timer is implemented. It disables the button after each invocation and displays a countdown until the button becomes clickable again.

## Usage

1. Configure **web.config** and **lambdafunctions.xml** as described above.
2. Deploy the application to your web server.
3. Access the application through a web browser.
4. Select a Lambda function from the dropdown menu.
5. Click the "Invoke" button to trigger the selected Lambda function.


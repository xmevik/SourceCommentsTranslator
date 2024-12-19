# Source Comments Translator

## Table of Contents

* [Description](#description)
* [Features](#features)
* [Options](#options)
* [Contributing](#contributing)
* [License](#license)
* [Contacts](#contacts)
* [Credits](#credits)

## Description

Source Comments Translator is a versatile tool designed to enhance your coding experience by translating comments in your source code files. Whether you're working on an open-source project, collaborating with international teams, or simply want to understand comments written in a different language, this tool is here to help.

## Features

	• Comment Translation: Automatically detects and translates comments in various programming languages within your source code files.

	• Multiple Translation Options: Choose between two powerful translation engines:

		 • LibreTranslate: An open-source translation API that respects your privacy.

		 • Google Translate: A widely-used translation service known for its accuracy and extensive language support.

	• Customizable Settings: Tailor the translation process to suit your needs with a variety of configuration options, including:

		• Choose the source language of your comments and the target language for translation. The application supports automatic detection of the source language.

		• Create a new file with the translated comments instead of overwriting the original file, preserving your initial code.
		
		• Utilize a customizable regex settings file to tailor comment detection according to your project•s needs.

		• Keep track of the translation process and any issues encountered through a dedicated log file.

		• Enable debug mode to receive detailed output for troubleshooting and understanding the translation process.

		• Choose whether to translate comments or skip them entirel.

		• Optionally use the Libre Translate API for translations, or any other translation engines, you can implement your own ITranslation interface.

		• Specify folders to ignore during the translation process, allowing you to focus on relevant parts of your project.

		• Translate only specific characters based on a regular expression pattern, giving you control over what gets translated.

## Options

- **--path** (required)
  - **Description**: Absolute path to your project.
  - **Example**: --path /path/to/your/project

- **--src** (optional, default is "auto")
  - **Description**: The source language of the comments.
  - **Example**: --src zt

- **--dest** (optional, default is "en")
  - **Description**: The target language for the comments.
  - **Example**: --dest es

- **--postscript** (optional)
  - **Description**: Creates a new file with [{postscript}{original filename}] instead of updating the existing file.
  - **Example**: --postscript updated_

- **--regpath** (optional, default is "./CommentRegex.json")
  - **Description**: Path to the file containing regex settings.
  - **Example**: --regpath /path/to/CommentRegex.json

- **--logpath** (optional, default is "./temp.log")
  - **Description**: Path to the log file.
  - **Example**: --logpath /path/to/logfile.log

- **--debug** (optional, default is false)
  - **Description**: Specifies whether to include debug information.
  - **Example**: --debug true

- **--donttranslate** (optional, default is false)
  - **Description**: Specifies whether to translate comments or not.
  - **Example**: --donttranslate true

- **-g, --libretranslate** (optional, default is false)
  - **Description**: Use the Libre Translate API.
  - **Example**: --libretranslate true

- **-r, --libretranslateuri** (optional, default is "http://localhost:5000/translate")
  - **Description**: URI endpoint for LibreTranslate.
  - **Example**: --libretranslateuri http://libretranslate.com/translate

- **-i, --ignorefolders** (optional, delimiter ",")
  - **Description**: Specify folders to ignore.
  - **Example**: --ignorefolders obj,bin

- **--sortcharsby** (optional, default is null)
  - **Description**: If you want to translate only specific characters (only accepts a regular expression pattern).
  - **Example**: --sortcharsby "[^a-zA-Z]$"

## Contributing

I'm welcome to contributions! If you'd like to contribute, please fork the repository and submit a pull request. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contacts

If you have any additional questions, here's how to reach me.

GitHub:[https://github.com/xmevik](https://github.com/xmevik)  
Email:[xmevik@vk.com](mailto:xmevik@vk.com)

## Credits

This README file was created by xmevik
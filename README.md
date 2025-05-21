ChatBot: Cybersecurity Awareness Assistant
**Overview**
ChatBot is a simple interactive C# console application designed to provide cybersecurity education and support. It simulates a conversation with a user, answering questions and offering personalized tips on topics like password security, phishing, malware, scams, and more. The bot detects user sentiment and adjusts responses to provide a more human and empathetic experience.

**Features**
💬 Conversational UI: Responds to natural language inputs and menu selections.

**Sentiment Detection:** Adjusts tone and responses based on emotional cues (e.g., confused, excited, scared).

**Cybersecurity Tips:** Offers guidance on safe passwords, scam awareness, malware protection, privacy, and social media safety.

**Audio Feedback:** Plays sound responses using .wav files.

**User Profile Tracking:** Remembers user interests, name, and recent topics to tailor the conversation.

**Interactive Menu:** Quick access to main topics via numbered selections.

**Error Handling:** Handles both critical and non-critical exceptions gracefully.

**How to Run**
**Prerequisites**
.NET SDK installed (version supporting C# 8.0 or higher)

A console terminal capable of running .NET applications

**Optional:** .wav audio file (welcome.wav) in the executable's working directory

**Steps**
Clone or download the repository.

Navigate to the project directory.

Build and run using the .NET CLI:

bash
Copy
Edit
dotnet build
dotnet run
📋 Menu Options
When the application starts, a menu is displayed:

**Option	Description**
1	Greet the user
2	Describe the chatbot's purpose
3	Password security tips
4	Phishing protection
5	Malware prevention
6	Safe browsing practices
7	Online privacy information
8	Social media security advice
9	Exit the application

Users can also type keywords or natural language questions.

**Sample Interactions**
plaintext
Copy
Edit
User: I'm really worried about malware.
Bot: It's completely understandable to feel that way about malware. Let me share some tips to help you feel more secure.
- Keep all software updated...
plaintext
Copy
Edit
User: Tell me more
Bot: Here’s more information about malware...
**File Structure**
ChatBot.cs: Main program file containing all logic

welcome.wav: Optional greeting audio

README.md: This file

👩‍💻 Technologies Used
Language: C#

Runtime: .NET Core / .NET 5+

**Libraries:**

System.Media for audio playback

System.Text.RegularExpressions for sentiment matching

**Future Improvements**
Add GUI interface for enhanced accessibility

Integrate text-to-speech for dynamic audio responses

Connect to external APIs for real-time security updates

Add multi-language support

**License**
This project is provided for educational use only. No warranties or guarantees are provided.

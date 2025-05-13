using System;
using System.Media;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ChatBot
{
    private static SoundPlayer responsePlayer;
    private static UserProfile currentUser = new UserProfile();

    #region Data Structures and Configuration

    // User profile class to store conversation context
    private class UserProfile
    {
        public string Name { get; set; } = "";
        public string FavoriteTopic { get; set; } = "";
        public string LastTopic { get; set; } = "";
        public bool NeedsMoreInfo { get; set; } = false;
        public string CurrentSentiment { get; set; } = "";
    }

    // Sentiment detection and responses
    private static readonly Dictionary<string, string> sentimentResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        {"worried|concerned|anxious|nervous", "It's completely understandable to feel that way about {0}. Let me share some tips to help you feel more secure."},
        {"frustrated|angry|annoyed", "I hear your frustration about {0}. Cybersecurity can be challenging, but we'll work through it together."},
        {"confused|unsure|puzzled", "{0} can be confusing at first. Let me break it down in simpler terms to help you understand."},
        {"overwhelmed|stressed", "I understand feeling overwhelmed by {0}. We'll take it one step at a time. You're doing great by seeking information!"},
        {"excited|interested|enthusiastic", "That's great you're excited about {0}! It's wonderful to see someone taking an active interest in their cybersecurity."},
        {"scared|afraid|fearful", "I understand being scared about {0}. The digital world can feel risky, but knowledge is your best protection."}
    };

    // Main keyword responses organized by topic
    private static readonly Dictionary<string, Action> keywordResponses = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
    {
        {"password", ShowPasswordTips},
        {"scam", ShowScamInfo},
        {"privacy", ShowPrivacyInfo},
        {"malware", ShowMalwareInfo},
        {"phishing", ShowPhishingInfo},
        {"browsing", ShowSafeBrowsingTips},
        {"social media", ShowSocialMediaSecurity}
    };

    // Menu options for numbered selection
    private static readonly Dictionary<int, Action> menuOptions = new Dictionary<int, Action>()
    {
        {1, () => Console.WriteLine("I'm great, thank you! How can I assist you today?")},
        {2, () => Console.WriteLine("I help you stay safe online by providing cybersecurity tips!")},
        {3, ShowPasswordTips},
        {4, ShowPhishingInfo},
        {5, ShowMalwareInfo},
        {6, ShowSafeBrowsingTips},
        {7, ShowPrivacyInfo},
        {8, ShowSocialMediaSecurity},
        {9, ExitApplication}
    };

    // Fallback responses for unrecognized inputs
    private static readonly List<string> fallbackResponses = new List<string>()
    {
        "I'm not sure I understand. Can you try rephrasing?",
        "I didn't catch that. Could you ask about cybersecurity topics like passwords, scams, or privacy?",
        "Let's focus on cybersecurity. Try asking about online safety topics.",
        "I specialize in cybersecurity. Ask me about staying safe online!"
    };

    #endregion

    #region Main Application Flow

    public static void Main(string[] args)
    {
        try
        {
            InitializeApplication();
            StartChatLoop();
        }
        catch (Exception ex)
        {
            HandleCriticalError(ex);
        }
    }

    private static void InitializeApplication()
    {
        Console.Clear();
        PlayVoiceGreeting("welcome.wav");
        DisplayImage();
        PrintSeparator();
        GreetUser();
        PrintSeparator();
    }

    private static void StartChatLoop()
    {
        while (true)
        {
            try
            {
                DisplayQuestionMenu();
                string userInput = GetUserInput();

                if (ShouldProcessInput(userInput))
                {
                    ProcessUserInput(userInput);
                }
            }
            catch (Exception ex)
            {
                HandleNonCriticalError(ex);
            }
        }
    }

    #endregion

    #region Core Functionality

    private static string GetUserInput()
    {
        string input = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Empty input received");
        }
        return input;
    }

    private static bool ShouldProcessInput(string input)
    {
        return !string.IsNullOrWhiteSpace(input);
    }

    private static void ProcessUserInput(string userInput)
    {
        userInput = userInput.ToLower();
        currentUser.CurrentSentiment = DetectSentiment(userInput);

        if (TryHandleMenuSelection(userInput)) return;
        if (TryHandleKeywordResponse(userInput)) return;
        if (TryHandleContinuation(userInput)) return;
        if (TryHandleExpressionOfInterest(userInput)) return;
        if (TryHandleConfusion(userInput)) return;

        ProvideFallbackResponse();
    }

    #endregion

    #region Response Handlers

    private static bool TryHandleMenuSelection(string input)
    {
        if (int.TryParse(input, out int selection) && menuOptions.ContainsKey(selection))
        {
            PlayResponseSound();
            menuOptions[selection].Invoke();
            return true;
        }
        return false;
    }

    private static bool TryHandleKeywordResponse(string input)
    {
        foreach (var keywordPair in keywordResponses)
        {
            if (input.Contains(keywordPair.Key))
            {
                PlayResponseSound();
                ProvideSentimentAwareIntroduction(keywordPair.Key);
                keywordPair.Value.Invoke();
                return true;
            }
        }
        return false;
    }

    private static bool TryHandleContinuation(string input)
    {
        if (currentUser.NeedsMoreInfo && (input.Contains("more") || input.Contains("explain") || input.Contains("details")))
        {
            ProvideExtendedInfo(currentUser.LastTopic);
            currentUser.NeedsMoreInfo = false;
            return true;
        }
        return false;
    }

    private static bool TryHandleExpressionOfInterest(string input)
    {
        if (input.Contains("interested in") || input.Contains("care about"))
        {
            foreach (var topic in keywordResponses.Keys)
            {
                if (input.Contains(topic))
                {
                    currentUser.FavoriteTopic = topic;
                    Console.WriteLine($"\nGreat, {currentUser.Name}! I'll remember you're interested in {topic}.");
                    Console.WriteLine("It's a crucial part of staying safe online.");
                    return true;
                }
            }
        }
        return false;
    }

    private static bool TryHandleConfusion(string input)
    {
        if (input.Contains("don't understand") || input.Contains("confused") || input.Contains("not clear"))
        {
            if (!string.IsNullOrEmpty(currentUser.LastTopic))
            {
                Console.WriteLine($"\nLet me try explaining {currentUser.LastTopic} differently...");
                ProvideExtendedInfo(currentUser.LastTopic);
                return true;
            }
        }
        return false;
    }

    private static void ProvideFallbackResponse()
    {
        Console.Beep(300, 500);
        Random rand = new Random();
        string response = fallbackResponses[rand.Next(fallbackResponses.Count)];

        if (!string.IsNullOrEmpty(currentUser.CurrentSentiment))
        {
            if (currentUser.CurrentSentiment.Contains("worried"))
            {
                response += " You might want to ask about protecting yourself from online threats.";
            }
            else if (currentUser.CurrentSentiment.Contains("confused"))
            {
                response += " Try asking about basic cybersecurity principles.";
            }
        }

        Console.WriteLine(response);
    }

    #endregion

    #region Information Display Methods

    private static void ShowPasswordTips()
    {
        currentUser.LastTopic = "password";
        Console.WriteLine("\n🔐 Password Security Tips:");
        Console.WriteLine("- Use at least 12 characters with mixed types (upper/lower case, numbers, symbols)");
        Console.WriteLine("- Never reuse passwords across different sites");
        Console.WriteLine("- Consider using a password manager like Bitwarden or LastPass");
        Console.WriteLine("- Enable two-factor authentication where available");
        OfferFollowUp("password");
    }

    private static void ShowScamInfo()
    {
        currentUser.LastTopic = "scam";
        Console.WriteLine("\n🚨 Scam Alert Information:");
        Console.WriteLine("- Common scam types: Phishing, Tech Support, Romance, Investment scams");
        Console.WriteLine("- Red flags: Urgency, threats, requests for payment in gift cards/crypto");
        Console.WriteLine("- Verify contacts through official channels before responding");
        OfferFollowUp("scam");
    }

    private static void ShowPrivacyInfo()
    {
        currentUser.LastTopic = "privacy";
        currentUser.FavoriteTopic = "privacy";
        Console.WriteLine("\n🛡️ Privacy Protection Guidelines:");
        Console.WriteLine("- Review privacy settings on all social media accounts monthly");
        Console.WriteLine("- Use encrypted messaging apps like Signal for sensitive communications");
        Console.WriteLine("- Be cautious about sharing location data and personal routines");
        OfferFollowUp("privacy");
    }

    private static void ShowMalwareInfo()
    {
        currentUser.LastTopic = "malware";
        Console.WriteLine("\n⚠️ Malware Protection Advice:");
        Console.WriteLine("- Keep all software updated, especially your operating system");
        Console.WriteLine("- Only download apps from official app stores/developer websites");
        Console.WriteLine("- Be extremely cautious with email attachments from unknown senders");
        OfferFollowUp("malware");
    }

    private static void ShowPhishingInfo()
    {
        currentUser.LastTopic = "phishing";
        Console.WriteLine("\n🎣 Phishing Protection:");
        Console.WriteLine("- Never click links in unsolicited emails or messages");
        Console.WriteLine("- Check sender email addresses carefully");
        Console.WriteLine("- Look for poor grammar and urgent requests");
        OfferFollowUp("phishing");
    }

    private static void ShowSafeBrowsingTips()
    {
        currentUser.LastTopic = "browsing";
        Console.WriteLine("\n🌐 Safe Browsing Practices:");
        Console.WriteLine("- Always check for HTTPS and padlock icon in address bar");
        Console.WriteLine("- Use a VPN on public Wi-Fi networks");
        Console.WriteLine("- Install browser security extensions");
        OfferFollowUp("safe browsing");
    }

    private static void ShowSocialMediaSecurity()
    {
        currentUser.LastTopic = "social media";
        Console.WriteLine("\n📱 Social Media Security:");
        Console.WriteLine("- Adjust privacy settings to limit visibility");
        Console.WriteLine("- Be selective about friend/follower requests");
        Console.WriteLine("- Think before you post - the internet never forgets");
        OfferFollowUp("social media");
    }

    #endregion

    #region Helper Methods

    private static void OfferFollowUp(string topic)
    {
        Console.WriteLine($"\nWould you like more details about {topic} protection? (yes/no)");
        try
        {
            var followUp = Console.ReadLine()?.ToLower();
            if (followUp?.StartsWith("y") == true)
            {
                currentUser.NeedsMoreInfo = true;
                ProvideExtendedInfo(topic);
            }
        }
        catch
        {
            Console.WriteLine("I'll assume you're done with this topic for now.");
        }
    }

    private static void ProvideSentimentAwareIntroduction(string topic)
    {
        if (!string.IsNullOrEmpty(currentUser.CurrentSentiment))
        {
            string response = GetSentimentResponse(currentUser.CurrentSentiment, topic);
            if (!string.IsNullOrEmpty(response))
            {
                Console.WriteLine("\n" + response + "\n");
            }
        }
    }

    private static void ProvideExtendedInfo(string topic)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        switch (topic.ToLower())
        {
            case "password":
                Console.WriteLine("\n💡 Advanced Password Tips:");
                Console.WriteLine("- Use passphrases (e.g., 'PurpleTiger$JumpsOver42Clouds!')");
                Console.WriteLine("- Check if your passwords have been compromised: haveibeenpwned.com");
                break;
            case "scam":
                Console.WriteLine("\n🔍 Deep Dive: Current Scam Trends");
                Console.WriteLine("- AI voice cloning scams targeting family members");
                Console.WriteLine("- Fake job offers requesting upfront payments");
                break;
            case "privacy":
                Console.WriteLine("\n🌐 Comprehensive Privacy Measures:");
                Console.WriteLine("- Use privacy-focused browsers like Firefox with strict tracking protection");
                Console.WriteLine("- Consider alternative search engines like DuckDuckGo");
                break;
            case "malware":
                Console.WriteLine("\n🛠️ Advanced Malware Protection:");
                Console.WriteLine("- Use sandbox environments for testing unknown files");
                Console.WriteLine("- Consider using a separate device for sensitive transactions");
                break;
            default:
                Console.WriteLine("\nHere's more detailed information:");
                break;
        }
        Console.ResetColor();

        if (!string.IsNullOrEmpty(currentUser.Name))
        {
            Console.WriteLine($"\n{currentUser.Name}, would you like me to explain any part of this in more detail?");
        }
    }

    private static string DetectSentiment(string input)
    {
        foreach (var sentimentPair in sentimentResponses)
        {
            if (Regex.IsMatch(input, $@"\b({sentimentPair.Key})\b"))
            {
                return sentimentPair.Key;
            }
        }
        return "";
    }

    private static string GetSentimentResponse(string sentimentPattern, string topic)
    {
        foreach (var pair in sentimentResponses)
        {
            if (pair.Key == sentimentPattern)
            {
                return string.Format(pair.Value, topic);
            }
        }
        return null;
    }

    #endregion

    #region UI Methods

    private static void DisplayQuestionMenu()
    {
        Console.WriteLine("\nYou can either:");
        Console.WriteLine("1. Type your question (e.g., 'tell me about phishing')");
        Console.WriteLine("2. Choose a number from the menu below:\n");

        for (int i = 1; i <= menuOptions.Count; i++)
        {
            string optionText = i switch
            {
                1 => "How are you?",
                2 => "What's your purpose?",
                3 => "Tell me about password safety",
                4 => "What should I know about phishing?",
                5 => "How can I protect against malware?",
                6 => "What are safe browsing practices?",
                7 => "How can I improve my online privacy?",
                8 => "What's important for social media security?",
                9 => "Exit",
                _ => ""
            };
            Console.WriteLine($"{i}. {optionText}");
        }

        if (!string.IsNullOrEmpty(currentUser.FavoriteTopic))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\n{currentUser.Name}, since you're interested in {currentUser.FavoriteTopic}, ");
            Console.WriteLine("you might want to ask about related topics!");
            Console.ResetColor();
        }

        Console.Write("\nEnter your question or the number of your choice: ");
    }

    private static void GreetUser()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Welcome to the Cybersecurity Awareness Bot!");
        Console.ResetColor();

        Console.Write("What's your name? ");
        try
        {
            currentUser.Name = Console.ReadLine()?.Trim() ?? "User";
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Hello, {currentUser.Name}! I'm here to help you stay safe online.");
            Console.ResetColor();
        }
        catch
        {
            currentUser.Name = "User";
            Console.WriteLine("Hello! I'm here to help you stay safe online.");
        }
    }

    private static void DisplayImage()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(@"
        
             )                (                )            
   (      ( /(     (          )\ )     (    ( /(     *   )  
   )\     )\())  ( )\   (    (()/(   ( )\   )\())  ` )  /(  
 (((_)   ((_)\   )((_)  )\    /(_))  )((_) ((_)\    ( )(_)) 
 )\___  __ ((_) ((_)_  ((_)  (_))   ((_)_    ((_)  (_(_())  
((/ __| \ \ / /  | _ ) | __| | _ \   | _ )  / _ \  |_   _|  
 | (__   \ V /   | _ \ | _|  |   /   | _ \ | (_) |   | |    
  \___|   |_|    |___/ |___| |_|_\   |___/  \___/    |_|    
                                                            

        ");
        Console.ResetColor();
    }

    private static void PrintSeparator()
    {
        Console.WriteLine(new string('-', 50));
    }

    #endregion

    #region Audio and System Methods

    private static void PlayVoiceGreeting(string audioFilePath)
    {
        try
        {
            if (File.Exists(audioFilePath))
            {
                using (SoundPlayer player = new SoundPlayer(audioFilePath))
                {
                    player.PlaySync();
                }
            }
        }
        catch
        {
            // Silent fallback
        }
    }

    private static void PlayResponseSound()
    {
        try
        {
            Console.Beep(1000, 200);
        }
        catch
        {
            // Silent fallback
        }
    }

    private static void ExitApplication()
    {
        PlayVoiceGreeting("goodbye.wav");
        Console.WriteLine("Goodbye! Stay safe online.");
        Environment.Exit(0);
    }

    #endregion

    #region Error Handling

    private static void HandleCriticalError(Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nA critical error occurred:");
        Console.WriteLine(ex.Message);
        Console.WriteLine("The application will now close.");
        Console.ResetColor();
        Environment.Exit(1);
    }

    private static void HandleNonCriticalError(Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nSomething went wrong, but we can continue:");
        Console.WriteLine(ex.Message);
        Console.WriteLine("Please try your question again.");
        Console.ResetColor();
    }

    #endregion
}
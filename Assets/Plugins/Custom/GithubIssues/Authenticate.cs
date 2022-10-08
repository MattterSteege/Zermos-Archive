/*
using System;
using System.Linq;
using System.Text;
using Octokit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Application = UnityEngine.Application;
using Random = UnityEngine.Random;

namespace GithubIssues
{
    public class Authenticate : MonoBehaviour
    {
        public static Authenticate Instance;
        
        private static string token = "";
        private static string username = "";
        private static string repoName = "";

        private GitHubClient _client;

        [SerializeField] private TMP_InputField title;
        [SerializeField] private TMP_InputField body;
        [SerializeField] private Button submit;
        [SerializeField] private TMP_Text submitText;
        [SerializeField] private GameObject module;
        [SerializeField] private TMP_Dropdown optionsDropdown;
        [SerializeField] private CanvasGroup mainCanvas;
        [SerializeField] private Texture2D screenshot;
        [SerializeField] private byte[] screenshotBase64;

        private Label[] _repoLabels;

        [SerializeField] private string imagePath;

        private void Awake()
        {
            _client = new GitHubClient(new ProductHeaderValue("Into-The-Dungeon"));

            var tokenAuth = new Credentials(token);
            _client.Credentials = tokenAuth;

            _repoLabels = _client.Issue.Labels.GetAllForRepository(username, repoName).Result.ToArray();

            Instance = this;

            foreach (var t in _repoLabels)
            {
                if (t.Name == "User Report") continue;

                optionsDropdown.options.Add(new TMP_Dropdown.OptionData(t.Name));
            }

            mainCanvas.gameObject.SetActive(false);
            mainCanvas.alpha = 0;
        }

        public void OnOpenIssueWindow()
        {
            screenshot = new Texture2D(Screen.width, Screen.height);
            screenshotBase64 = screenshot.EncodeToPNG();

            mainCanvas.gameObject.SetActive(!module.activeInHierarchy);
        }

        public void SubmitIssue()
        {
            if (title.text == string.Empty || body.text == string.Empty)
            {
                submitText.color = new Color(1, 0, 0, 1);
                submitText.text = "Please fill in all fields";
                return;
            }

            submit.interactable = false;
            submitText.text = "Submitting your issue";
            string image = Convert.ToBase64String(screenshotBase64);

            CreateFileRequest newFile = new CreateFileRequest(
                DateTime.UtcNow.AddHours(1).ToString("yyyy_MM_dd_HH_mm_ss") + "_" + Random.Range(0, 100000), image,
                "Reports", false);

            _client.Repository.Content.CreateFile(username, repoName, $"Screenshots/{newFile.Message}.png", newFile);

            imagePath =
                $"https://github.com/{username}/{repoName}/blob/Reports/Screenshots/{newFile.Message}.png?raw=true";

            print(imagePath);

            var createIssue =
                new NewIssue($"[Bug Report] {title.text} ({username} - {DateTime.UtcNow.AddHours(1):f})");

            var bodyText = "";

            bodyText += "## User bug report\n\n";
            bodyText += "## Description\n\n";
            bodyText += $"{body.text}\n\n";
            bodyText += "\n\n";
            bodyText += $"App Version {Application.version}\n\n";
            bodyText += "\n\n";
            bodyText += "## Error Message:\n\n";
            bodyText += "\n\n";
            bodyText += "\n\n";
            bodyText += $"![BugShot]({imagePath})";
            bodyText += "\n\n";
            //bodyText += AppendLogFile();

            createIssue.Body = bodyText;

            createIssue.Assignees.Add(username);
            createIssue.Labels.Add("User Report");
            createIssue.Labels.Add(_repoLabels[optionsDropdown.value].Name);


            var issue = _client.Issue.Create(username, repoName, createIssue);

            submitText.text = "Thank you for submitting your issue";

            mainCanvas.gameObject.SetActive(false);

            print("Created issue at: " + issue.Result.HtmlUrl);

            var sb = new StringBuilder("---");
            sb.AppendLine();
            sb.AppendLine("date: \"2021-05-01\"");
            sb.AppendLine("title: \"My new fancy post\"");
            sb.AppendLine("tags: [csharp, azure, dotnet]");
            sb.AppendLine("---");
            sb.AppendLine();

            sb.AppendLine("# The heading for my first post");
            sb.AppendLine();
        }
    }
}*/
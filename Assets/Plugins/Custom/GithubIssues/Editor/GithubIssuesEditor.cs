using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Octokit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Application = UnityEngine.Application;
using Button = UnityEngine.UIElements.Button;
using Label = Octokit.Label;

namespace GithubIssues
{
    public class GithubIssuesEditor : EditorWindow
    {
        private static string _token = "";
        private static string _username = "";
        private static string _repoName = "";

        private int _mSelectedIndex = -1;
        private VisualElement _lowerPane;
        private TwoPaneSplitView _splitView;

        private string _issueTitle = "";
        private string _issueBody = "";

        private static GitHubClient _client;

        private static List<Label> _repoLabels;
        private static List<string> _issueTypes;

        private TextField _titleInputField;
        private TextField _descriptionInputField;
        private DropdownField _issueDropdown;
        private VisualElement _submitButton;
        private Issue[] _issues;
        private ListView _upperPaneList;


        [MenuItem("Github For Unity/Github Issues")]
        public static void ShowExample()
        {
            var wnd = GetWindow<GithubIssuesEditor>();
            wnd.titleContent = new GUIContent("Github Issues Editor");
        }

        public void CreateGUI()
        {
            SettingsScriptableObject settingsScriptableObject =
                AssetDatabase.LoadAssetAtPath<SettingsScriptableObject>(
                    "Assets/Plugins/Custom/GithubIssues/Editor/Settings.asset");
            _token = settingsScriptableObject.token;

            _username = settingsScriptableObject.repoURL.Replace("https://", "").Trim().Split('/')[1];
            _repoName = settingsScriptableObject.repoURL.Replace("https://", "").Trim().Split('/')[2];

            _client = new GitHubClient(new ProductHeaderValue(_repoName ?? "Unity-Game"));
            
            _client.Credentials = new Credentials(_token);

            #region UpperPane

            _issueTypes = new List<string>();

            _repoLabels = _client.Issue.Labels.GetAllForRepository(_username, _repoName).Result.ToList();
            _issueTypes = _repoLabels.Select(x => x.Name).ToList();

            //------------------------------ Title --------------------------------------------------

            //VisualElement root = rootVisualElement;

            VisualElement enterTitle = new UnityEngine.UIElements.Label("Enter title Here");
            if (enterTitle == null) throw new ArgumentNullException(nameof(enterTitle));
            enterTitle.style.alignSelf = Align.Center;

            _titleInputField = new TextField();

            rootVisualElement.Add(enterTitle);
            rootVisualElement.Add(_titleInputField);

            //------------------------------ Description --------------------------------------------------

            VisualElement enterDescription = new UnityEngine.UIElements.Label("Enter description Here");
            enterDescription.style.alignSelf = Align.Center;

            _descriptionInputField = new TextField
            {
                style =
                {
                    height = 200
                },
                multiline = true
            };
            enterDescription.style.whiteSpace = WhiteSpace.Normal;

            rootVisualElement.Add(enterDescription);
            rootVisualElement.Add(_descriptionInputField);

            //------------------------------ Dropdown for Issue Types -----------------------------------------------------------

            VisualElement enterIssueType = new UnityEngine.UIElements.Label("Enter Issue Type Here");
            enterIssueType.style.alignSelf = Align.Center;

            _issueDropdown = new DropdownField("Issue Type", _issueTypes, 0);

            rootVisualElement.Add(enterIssueType);
            rootVisualElement.Add(_issueDropdown);


            //------------------------------ Submit Button -----------------------------------------------------------

            _submitButton = new Button(() =>
            {
                _issueTitle = _titleInputField.value;
                _issueBody = _descriptionInputField.value;
                //IssueType = IssueDropdown; //figure out how to get the value of the dropdown


                SubmitIssue();
            })
            {
                text = "Submit Issue"
            };

            rootVisualElement.Add(_submitButton);

            /*
             * different tabs for adding or viewing issues
             */

            #endregion

            #region divider

            var divider = new VisualElement
            {
                style =
                {
                    height = 5,
                    backgroundColor = new StyleColor(new Color(56f / 255f, 56f / 255f, 56f / 255f))
                }
            };

            var divider1 = new VisualElement
            {
                style =
                {
                    height = 1,
                    backgroundColor = Color.grey
                }
            };

            rootVisualElement.Add(divider);

            rootVisualElement.Add(divider1);

            var refreshButton = new Button(() =>
            {
                // Clear all previous content from the pane
                _lowerPane.Clear();

                _issues = _client.Issue.GetAllForRepository(_username, _repoName).Result.ToArray();

                _upperPaneList.makeItem = () => new UnityEngine.UIElements.Label();
                _upperPaneList.bindItem = (item, index) =>
                {
                    ((UnityEngine.UIElements.Label) item).text = _issues[index].Title;
                };
                _upperPaneList.itemsSource = _issues;
            })
            {
                text = "Refresh"
            };

            rootVisualElement.Add(refreshButton);

            rootVisualElement.Add(divider1);

            rootVisualElement.Add(divider);

            #endregion

            #region LowerPane

            _splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Vertical);
            rootVisualElement.Add(_splitView);

            _issues = _client.Issue.GetAllForRepository(_username, _repoName).Result.ToArray();

            _upperPaneList = new ListView
            {
                selectionType = SelectionType.Single
            };

            _splitView.Add(_upperPaneList);

            _lowerPane = new VisualElement
            {
                style =
                {
                    minHeight = 300,
                    maxHeight = 300
                }
            };
            _splitView.Add(_lowerPane);

            _upperPaneList.makeItem = () => new UnityEngine.UIElements.Label();
            _upperPaneList.bindItem = (item, index) =>
            {
                ((UnityEngine.UIElements.Label) item).text = _issues[index].Title;
            };
            _upperPaneList.itemsSource = _issues;

            // React to the user's selection
            _upperPaneList.onSelectionChange += OnFetchDifferentIssue;

            // Restore the selection index from before the hot reload
            _upperPaneList.selectedIndex = _mSelectedIndex;

            // Store the selection index when the selection changes
            _upperPaneList.onSelectionChange += _ => { _mSelectedIndex = _upperPaneList.selectedIndex; };

            #endregion
        }

        private void OnFetchDifferentIssue(IEnumerable<object> selectedItems)
        {
            _upperPaneList.Clear();

            // Clear all previous content from the pane
            _lowerPane.Clear();

            _issues = _client.Issue.GetAllForRepository(_username, _repoName).Result.ToArray();

            if (_issues == null || _issues == Array.Empty<Issue>()) return;

            _upperPaneList.makeItem = () => new UnityEngine.UIElements.Label();

            _upperPaneList.bindItem = (item, index) =>
            {
                try
                {
                    ((UnityEngine.UIElements.Label) item).text = _issues[index].Title;
                }
                catch (IndexOutOfRangeException) { }
            };

            _upperPaneList.itemsSource = _issues;

            // Get the selected sprite

            // Add a new Image control and display the sprite

            if (selectedItems.First() is Issue selectedIssue)
            {
                var issueTitle = new UnityEngine.UIElements.Label(selectedIssue.Title)
                {
                    style =
                    {
                        marginTop = 2,
                        marginBottom = 2,
                        alignSelf = Align.Center,
                        textOverflow = TextOverflow.Ellipsis
                    }
                };

                var issueBody = new TextField();
                issueTitle.style.marginTop = 2;
                issueTitle.style.marginBottom = 2;
                issueBody.style.height = 200;
                issueBody.multiline = true;
                issueBody.value = selectedIssue.Body?.Replace("## User bug report", "---USER BUG REPORT---")
                    .Replace("## Description", "---DESCRIPTION---") ?? issueTitle.text;
                issueBody.style.whiteSpace = WhiteSpace.Normal;

                var issueTypes = new TextField
                {
                    style =
                    {
                        marginTop = 2,
                        marginBottom = 2,
                        alignSelf = Align.Center,
                        textOverflow = TextOverflow.Ellipsis
                    },
                    value = "No Labels Specified"
                };
                
                if (!(string.Join(", ", selectedIssue.Labels.Select(x => x.Name).ToArray()).Replace(" ", "") == "" || string.Join(", ", selectedIssue.Labels.Select(x => x.Name).ToArray()).Replace(" ", "") == String.Empty))
                {
                    issueTypes.value = string.Join(", ", selectedIssue.Labels.Select(x => x.Name).ToArray());
                }

                if (selectedIssue.Comments > 0)
                {
                    issueTypes.value += $" (+ {selectedIssue.Comments} comment{(selectedIssue.Comments > 1 ? "s" : "")})";
                }

                var linkButton = new Button(() => { Application.OpenURL(selectedIssue.HtmlUrl); })
                {
                    text = "Open Issue on Github"
                };

                var divider = new VisualElement
                {
                    style =
                    {
                        height = 5,
                        backgroundColor = new StyleColor(new Color(56f / 255f, 56f / 255f, 56f / 255f))
                    }
                };

                var closeButton = new Button(() =>
                {
                    _client.Issue.Update(_username, _repoName, selectedIssue.Number, new IssueUpdate
                    {
                        State = ItemState.Closed
                    });
                    _client.Issue.Comment.Create(_username, _repoName, selectedIssue.Number, _descriptionInputField.value ?? "This has been resolved!");
                })
                {
                    text = "Mark Issue closed"
                };

                var CommentButton = new Button(() =>
                {
                    if(string.IsNullOrEmpty(_descriptionInputField.value) || _descriptionInputField.value == "")
                    {
                        _descriptionInputField.value = "Enter Comment here";
                    }
                    else
                    {
                        _client.Issue.Comment.Create(_username, _repoName, selectedIssue.Number, _descriptionInputField.value ?? "This has been resolved!");
                        _descriptionInputField.value = "Done";
                    }
                })
                {
                    text = "Add comment (description field)"
                };

                // Add the Image control to the right-hand pane
                _lowerPane.Add(issueTitle);
                _lowerPane.Add(issueBody);
                _lowerPane.Add(issueTypes);
                _lowerPane.Add(linkButton);
                _lowerPane.Add(divider);
                _lowerPane.Add(closeButton);                
                _lowerPane.Add(divider);
                _lowerPane.Add(CommentButton);
            }
        }

        private void SubmitIssue()
        {
            if (_issueTitle == string.Empty || _issueBody == string.Empty) return;

            _titleInputField.value = "";
            _descriptionInputField.value = "Submitting...";

            var createIssue = new NewIssue(_issueTitle);
            createIssue.Assignees.Add(_username);

            createIssue.Labels.Add(_issueTypes[_issueDropdown.index]);

            createIssue.Body = _issueBody;

            _client.Issue.Create(_username, _repoName, createIssue);

            _descriptionInputField.value = "Successfully Submitted!";
            
            Thread.Sleep(1000);
            
            // Clear all previous content from the pane
            _lowerPane.Clear();

            _issues = _client.Issue.GetAllForRepository(_username, _repoName).Result.ToArray();

            _upperPaneList.makeItem = () => new UnityEngine.UIElements.Label();
            _upperPaneList.bindItem = (item, index) =>
            {
                ((UnityEngine.UIElements.Label) item).text = _issues[index].Title;
            };
            _upperPaneList.itemsSource = _issues;
        }
    }
}
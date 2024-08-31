using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace SA.EasyConsole
{
    [ExecuteAlways]
    public class EasyConsole : MonoBehaviour
    {
#region InspectorFields

        [SerializeField] private KeyCode openConsoleKey = KeyCode.BackQuote;
        [SerializeField] private int historySize = 50;
        [SerializeField] private bool dontDestroyOnLoad = true;
        [SerializeField] private bool activeOnLoad;
        
        [Space(7)]
        
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI logText;
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private GameObject container;

#endregion


#region Fields

        public static event Action OnLogged;
        
        private static EasyConsole _instance;
        
        private static CommandObject[] _commands = 
        {
            new(typeof(SceneManager).GetMethod("LoadScene", new []{ typeof(string) })) { Path = "LoadScene" },
            new(typeof(EasyConsole).GetMethod("ClearConsole")) { Path = "Clear" }
        };

        private List<string> _inputHistory = new();
        private int _chosenInHistoryIndex;
        private string _inputBeforeUsingHistory;
        
        private bool _isConsoleOpened;

#endregion

        private void Awake()
        {
            if(!Application.isPlaying) return;
            
            if(_instance != null)
                Destroy(transform.parent.gameObject);
            
            _instance = this;

            if(dontDestroyOnLoad)
                DontDestroyOnLoad(transform.parent);
            
            LoadCommands();
            inputField.onValueChanged.AddListener(UpdateHints);
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                (activeOnLoad ? (Action)ActivateConsole : DisableConsole).Invoke();
                return;
            }
            
            HandleInputs();
        }

        private void OnDestroy() => _instance = null;
        private void OnApplicationQuit() => _instance = null;
        
        
#region Logger

        public static void LogLine(object message, bool printToUnityConsole = true) =>
            Log(message + "\n", printToUnityConsole);

        public static void Log(object message, bool printToUnityConsole = true)
        {
            if (printToUnityConsole)
                Debug.Log(message);

            _instance.logText.text += message;
            
            OnLogged?.Invoke();
        }

        public static void LogError(object message, bool printToUnityConsole = true)
        {
            if(printToUnityConsole)
                Debug.LogError(message);
            
            _instance.logText.text += $"<color=#ff0000>{message}</color>\n";
            
            OnLogged?.Invoke();
        }

        public static void ClearConsole()
        {
            _instance.logText.text = "";
            
            OnLogged?.Invoke();
        }

#endregion


#region ConsoleStatus

        public void ActivateConsole()
        {
            container.SetActive(true);

            inputField.ActivateInputField();
            inputField.Select();
            inputField.text = "";
        }

        public void DisableConsole()
        {
            container.SetActive(false);

            inputField.DeactivateInputField();
        }
        
        private void ChangeConsoleStatus()
        {
            _isConsoleOpened = !_isConsoleOpened;
            
            if(_isConsoleOpened)
                ActivateConsole();
            else
                DisableConsole();
        }

#endregion

        private void EnterCommand()
        {
            _chosenInHistoryIndex = 0;
            _inputBeforeUsingHistory = "";
            
            LogLine($"<color=#00ff00>{GetTime()}:</color> {inputField.text}", false);
            
            if(inputField.text == "")
                return;
            
            var input = inputField.text.Split(" ").ToList();
            input.Remove("");
            
            _inputHistory.Insert(0, inputField.text);
            if (_inputHistory.Count > historySize)
                _inputHistory.RemoveAt(_inputHistory.Count - 1);
            
            foreach (var command in _commands)
            {
                if(command.Path != input[0]) continue;

                try
                {
                    var rawInput = new string[input.Count - 1];
                    Array.Copy(input.ToArray(), 1, rawInput, 0, rawInput.Length);

                    var parameters = new object[] {};
                    
                    switch (command.Type)
                    {
                        case CommandObject.CommandType.Method:
                            parameters = ParseParameters(rawInput, command.Method.GetParameters());
                            break;
                        
                        case CommandObject.CommandType.Field:
                            if (rawInput.Length > 0)
                                parameters = new [] { ParseType(command.Field.FieldType, rawInput[0]) };
                            break;
                        
                        case CommandObject.CommandType.Property:
                            if (rawInput.Length > 0)
                                parameters = new[] { ParseType(command.Property.PropertyType, rawInput[0]) };
                            break;
                    }
                    
                    switch (command.Type)
                    {
                        case CommandObject.CommandType.Method:
                            var result = command.Method.Invoke(command.Instance, parameters);
                            if(result != null)
                                LogLine(result);
                            break;
                        
                        case CommandObject.CommandType.Field:
                            if(parameters.Length > 0)
                                command.Field.SetValue(command.Instance, parameters[0]);
                            else
                                LogLine($"{command.Path} = {command.Field.GetValue(command.Instance)}");
                            break;
                        
                        case CommandObject.CommandType.Property:
                            if(parameters.Length > 0)
                                command.Property.SetValue(command.Instance, parameters[0]);
                            else
                                LogLine($"{command.Path} = {command.Property.GetValue(command.Instance)}");
                            break;
                    }
                }
                catch(Exception e)
                {
                    LogError(e);
                }
                
                break;
            }
            
            inputField.text = "";
        }

        
#region Input

        private void HandleInputs()
        {
            if (Input.GetKeyDown(openConsoleKey))
                ChangeConsoleStatus();
            
            if(!_isConsoleOpened)
                return;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (_chosenInHistoryIndex == 0)
                    _inputBeforeUsingHistory = inputField.text;
                
                _chosenInHistoryIndex = Mathf.Clamp(_chosenInHistoryIndex + 1, 0, _inputHistory.Count);
                UpdateInputByHistory();
            }
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _chosenInHistoryIndex = Mathf.Clamp(_chosenInHistoryIndex - 1, 0, _inputHistory.Count);
                UpdateInputByHistory();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
                EnterCommand();
                inputField.ActivateInputField();
            }
            
            if(Input.GetKeyDown(KeyCode.Tab))
                UseHint();
        }
        
        private void UpdateInputByHistory()
        {
            inputField.text = _chosenInHistoryIndex == 0 ? 
                _inputBeforeUsingHistory : 
                _inputHistory[_chosenInHistoryIndex - 1];
            
            inputField.text = inputField.text.Replace("\n", "");
        }

#endregion


#region Hints

        private void UseHint()
        {
            var input = inputField.text.Split(" ");
            if(input.Length > 1)
                return;

            var foundResult = FindCommand(input[0]);
            if(foundResult == null)
                return;

            inputField.text = foundResult.Path;
            inputField.caretPosition = inputField.text.Length;
        }
        
        private void UpdateHints(string inputRaw)
        {
            var input = inputRaw.Split(" ").ToList();
            var findResult = FindCommand(input[0]);
            input.Remove("");
            
            if (findResult == null)
            {
                hintText.text = "";
                return;
            }

            var resultText = "";

            if (input.Count < 2)
            {
                resultText = findResult.Path;
                
                switch (findResult.Type)
                {
                    case CommandObject.CommandType.Method:
                        foreach (var parameter in findResult.Method.GetParameters())
                            resultText += ConvertToParameter(parameter.ParameterType);
                        break;
                
                    case CommandObject.CommandType.Field:
                        resultText += ConvertToParameter(findResult.Field.FieldType);
                        break;
                
                    case CommandObject.CommandType.Property:
                        resultText += ConvertToParameter(findResult.Property.PropertyType);
                        break;
                }
            }
            else if (findResult.Type == CommandObject.CommandType.Method)
            {
                resultText = inputRaw;
                
                var parameters = findResult.Method.GetParameters();
                if (input.Count <= parameters.Length)
                {
                    var parametersToShow = parameters.Skip(input.Count - 1).ToArray();
                    foreach (var parameter in parametersToShow)
                        resultText += ConvertToParameter(parameter.ParameterType);
                }
            }

            hintText.text = resultText;
            
            return;
            string ConvertToParameter(Type t) =>
                $" <color=#00FFFF>[{t.Name}]</color>";
        }

#endregion


#region Utilities

        private CommandObject FindCommand(string input) =>
            input == "" ? null : _commands.FirstOrDefault(command => command.Path.StartsWith(input));
                
        private static object[] ParseParameters(string[] input, ParameterInfo[] requestedParameters)
        {
            var length = (input.Length < requestedParameters.Length ? input.Length : requestedParameters.Length);
            var result = new object[length];
                    
            for (var i = 0; i < length; i++)
                result[i] = ParseType(requestedParameters[i].ParameterType, input[i]);

            return result;
        }
                
        private static object ParseType(Type type, string value)
        {
            switch (type)
            {
                case var _ when type == typeof(string):
                    return value;
                                    
                case var _ when type == typeof(int):
                    return int.Parse(value);
                                    
                case var _ when type == typeof(bool):
                    return value.ToLower() == "true";
                                    
                case var _ when type == typeof(double):
                    return double.Parse(value);
                                    
                case var _ when type == typeof(float):
                    return float.Parse(value);
                                    
                default:
                    LogError("Unknown requested type");
                    return null;
            }
        }

        private static string GetTime()
        {
            var timeSpan = TimeSpan.FromSeconds(Time.time);
            return $"[{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}:{timeSpan.Milliseconds:D3}]";
        }
 
#endregion


#region CommandsLoading

        private void LoadCommands()
        {
            const BindingFlags bindingAttributes = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var commands = new List<CommandObject>();
                    
            foreach (var type in types)
            {
                commands.AddRange(
                    from method in type.GetMethods(bindingAttributes)
                    let attribute = method.GetCustomAttribute<Command>()
                    where attribute != null
                    select new CommandObject(method));

                commands.AddRange(
                    from field in type.GetFields(bindingAttributes)
                    let attribute = field.GetCustomAttribute<Command>()
                    where attribute != null
                    select new CommandObject(field, attribute.Path));

                commands.AddRange(
                    from parameter in type.GetProperties(bindingAttributes)
                    let attribute = parameter.GetCustomAttribute<Command>()
                    where attribute != null
                    select new CommandObject(parameter, attribute.Path));
            }

            _commands = _commands.Concat(commands).ToArray();
        }

        public static void RegisterCommands(CommandObject[] commands) =>
            _commands = _commands.Concat(commands).ToArray();
        
        public static void RegisterCommand(CommandObject command)
        {
            Array.Resize(ref _commands, _commands.Length + 1);
            _commands[^1] = command;
        }

        public static void RemoveCommands(CommandObject[] commands)
        {
            foreach (var command in commands)
                RemoveCommand(command);
        }
        
        public static void RemoveCommand(CommandObject command) => 
            _commands = Array.FindAll(_commands, val => val != command);
        
        
        #endregion
    }
}

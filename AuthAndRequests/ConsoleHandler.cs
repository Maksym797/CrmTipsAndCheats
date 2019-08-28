using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthAndRequests
{
    public static class ConsoleHandler
    {
        #region Answer from list

        public static bool ConfirmYesNoStatement(string prompt)
        {
            return ChooseOptionFromList(new List<string> { "Yes", "No" }, new ConsoleListConfig { Prompt = prompt })
                   == "Yes";
        }

        public static List<string> ChooseMultipleOptionsFromList(IEnumerable<string> options, ConsoleListConfig config = null)
        {
            var names = options as List<string> ?? options.ToList();
            var result = new List<string>();

            if (config?.Prompt != null)
            {
                Console.WriteLine(config.Prompt);
            }

            var selector = config?.Selector ?? "√";
            var selectorSpace = new string(' ', selector.Length);
            var topList = Console.CursorTop;
            var position = 0;
            var applyAnswer = config?.ApplyAnswer ?? "[Apply]";
            var selectAllAnswer = config?.SelectAllAnswer ?? "[Select All]";

            names.Add(selectAllAnswer);
            names = MoveValueFromTop(names, selectAllAnswer);
            names.Add(applyAnswer);

            names.ForEach(s => Console.WriteLine(selectorSpace + s));

            Console.CursorTop = topList;
            Console.Write($"{selectorSpace}{names[position]}");

            Console.CursorLeft = 0;
            while (true)
            {
                var input = Console.ReadKey();
                var isAlreadyContain = result.Contains(names[position]);

                Console.CursorLeft = 0;
                Console.Write($"{(isAlreadyContain ? selector : selectorSpace)}{names[position]}{selectorSpace}");
                switch (input.Key)
                {
                    case ConsoleKey.DownArrow:
                        {
                            InputKeyHandler(ref position, position < names.Count - 1, true);
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        {
                            InputKeyHandler(ref position, position > 0, false);
                        }
                        break;
                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        {
                            if (names[position] == applyAnswer)
                            {
                                CleanList(names, topList, selector, config?.DefaultAnswerPostfix);
                                Console.WriteLine($"{config?.BeforeAnswer ?? "Selected: "}{string.Join(config?.MultipleResultSeparator ?? ", ", result)}{config?.AfterAnswer}");
                                return result;
                            }
                            if (names[position] == selectAllAnswer)
                            {
                                Console.CursorTop = topList + 1;
                                Console.CursorLeft = 0;
                                SelectAllAnswerHandler(names, result, applyAnswer, selectAllAnswer);
                                result.ForEach(a => Console.WriteLine(selector + a));
                                position = result.Count + 1;
                                break;
                            }
                            if (isAlreadyContain)
                            {
                                result.Remove(names[position]);
                                break;
                            }
                            result.Add(names[position]);
                        }
                        break;
                }
                Console.CursorLeft = 0;
                Console.Write($"{(result.Contains(names[position]) ? selector : selectorSpace)}{names[position]}");
                Console.CursorLeft = 0;
            }
        }

        private static void SelectAllAnswerHandler(List<string> names, List<string> results, params string[] exceptAnswers)
        {
            results.Clear();
            results.AddRange(names);
            exceptAnswers.ToList().ForEach(a => results.Remove(a));
        }

        public static string ChooseOptionFromList(IEnumerable<string> options, ConsoleListConfig config = null)
        {
            var names = options as List<string> ?? options.ToList();

            if (config?.Prompt != null)
            {
                Console.WriteLine(config.Prompt);
            }

            names = MoveDefaultValueFromTop(names, config);

            var selector = config?.Selector ?? ">";
            var topList = Console.CursorTop;
            var position = 0;

            names.ForEach(Console.WriteLine);

            Console.CursorTop = topList;
            Console.Write($"{selector}{names[position]}");

            Console.CursorLeft = 0;
            while (true)
            {
                var input = Console.ReadKey();

                Console.CursorLeft = 0;
                Console.Write($"{names[position]}{new string(' ', selector.Length)}");
                switch (input.Key)
                {
                    case ConsoleKey.DownArrow:
                        {
                            InputKeyHandler(ref position, position < names.Count - 1, true);
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        {
                            InputKeyHandler(ref position, position > 0, false);
                        }
                        break;
                    case ConsoleKey.Enter:
                        {
                            if (names[position] != config?.DisabledAnswer)
                            {
                                names = ResetDefaultValue(names, config);
                                CleanList(names, topList, selector, config?.DefaultAnswerPostfix);
                                var result = string.IsNullOrEmpty(config?.DefaultAnswerPostfix)
                                    ? names[position]
                                    : names[position].Replace(config.DefaultAnswerPostfix, "");
                                Console.WriteLine($"{config?.BeforeAnswer}{result}{config?.AfterAnswer}");
                                return result;
                            }
                        }
                        break;
                }
                Console.CursorLeft = 0;
                Console.Write($"{selector}{names[position]}");
                Console.CursorLeft = 0;
            }
        }

        private static void InputKeyHandler(ref int position, bool statement, bool isIncrement)
        {
            var deltaValue = isIncrement ? 1 : -1;

            if (statement)
            {
                position += deltaValue;
                Console.CursorTop += deltaValue;
            }
            else
            {
                Console.Beep();
            }
        }

        private static List<string> MoveDefaultValueFromTop(List<string> list, ConsoleListConfig config)
        {
            if (config?.DefaultAnswer == null)
            {
                return list;
            }

            string defaultValue = config.DefaultAnswer;
            string postfix = config.DefaultAnswerPostfix ?? "";

            return MoveValueFromTop(list, defaultValue, postfix);
        }
        private static List<string> MoveValueFromTop(List<string> list, string value, string postfix = null)
        {
            var result = new List<string> { list.FirstOrDefault(a => a == value) + postfix };

            if (result.Count == 0)
            {
                throw new Exception("Can`t move default to top of list.");
            }

            list.ForEach(e => { if (e != value) result.Add(e); });

            return result;
        }

        private static List<string> ResetDefaultValue(List<string> list, ConsoleListConfig config)
        {
            if (config?.DefaultAnswerPostfix == null)
            {
                return list;
            }

            return list.Select(e => e.Contains(config.DefaultAnswerPostfix)
                    ? e.Replace(config.DefaultAnswerPostfix, "")
                    : e)
                    .ToList();
        }

        private static void CleanList(List<string> list, int topList, string selector, string postfix)
        {
            selector = selector ?? new string(' ', 5);
            postfix = postfix ?? "";
            Console.CursorTop = topList;
            Console.CursorLeft = 0;
            list.ForEach(e => Console.WriteLine(new string(' ', e.Length + selector.Length + postfix.Length)));
            Console.CursorTop = topList;
        }
        #endregion

        #region Text answer
        public static string ResponseFromCommandLine(string promptText)
        {
            Console.WriteLine($"{promptText}:");
            return Console.ReadLine()?.Trim();
        }
        #endregion

        #region Process lables

        public static void ShowLoading(Task waitFor, bool isLockedInput = true)
        {
            var baseLabel = "Loading";
            var completedMessage = "Completed";
            var maxPointCount = 5;
            var sleepInterval = 100;
            var baseSymbol = ".";

            Console.Write(baseLabel);

            for (var i = 0; i <= maxPointCount; i++)
            {
                if (i == maxPointCount)
                {
                    Console.CursorLeft -= maxPointCount;
                    Console.Write(new string(' ', maxPointCount));
                    Console.CursorLeft -= maxPointCount;
                    i = 0;
                }

                Console.Write(baseSymbol);
                if (waitFor.IsFaulted)
                {
                    throw waitFor.Exception?.InnerExceptions.LastOrDefault() ?? new Exception("Unknown exception in background task!");
                }
                if (waitFor.IsCompleted)
                {
                    Console.CursorLeft = 0;
                    Console.Write(new string(' ', baseLabel.Length + ++i));
                    Console.CursorLeft = 0;
                    Console.WriteLine(completedMessage);
                    return;
                }

                Thread.Sleep(sleepInterval);
            }
        }

        public static void WriteLine(string message, ConsoleColor color)
        {
            var currCol = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = currCol;
        }

        #endregion
    }
}

// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project. 
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc. 
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File". 
// You do not need to add suppressions to this file manually. 

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Yasc")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Yasc", Scope = "namespace", Target = "Yasc")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Yasc", Scope = "namespace", Target = "Yasc.AI")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Yasc", Scope = "namespace", Target = "Yasc.Gui")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lvs", Scope = "member", Target = "Yasc.Gui.LvsSettingsExtension.#LoadLvs(System.Configuration.SettingsBase)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lvs", Scope = "type", Target = "Yasc.Gui.LvsSettingsExtension")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lvs", Scope = "member", Target = "Yasc.Gui.LvsSettingsExtension.#SaveLvs(System.Configuration.SettingsBase,System.Collections.ObjectModel.ObservableCollection`1<System.String>,System.String)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lvs", Scope = "member", Target = "Yasc.Gui.LvsSettingsExtension.#SaveLvs(System.Configuration.SettingsBase,System.Collections.Generic.IList`1<System.String>)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "l", Scope = "member", Target = "Yasc.Networking.FuncListener`2.#op_Implicit(Yasc.Networking.FuncListener`2<!0,!1>):System.Func`2<!0,!1>")]

[assembly: SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Yasc.ChatCommands.#SendMessage")]
[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Yasc.EntryPoint.#CurrentDomainOnUnhandledException(System.Object,System.UnhandledExceptionEventArgs)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Yasc.EntryPoint.#SaveToFile(System.Object)")]
[assembly: SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Scope = "member", Target = "Yasc.MainWindowViewModel.#Dispose()")]
[assembly: SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "member", Target = "Yasc.MainWindowViewModel.#Dispose()")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ai", Scope = "type", Target = "Yasc.AI.AiControllerBase")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ai", Scope = "type", Target = "Yasc.AI.CleverAiController")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ai", Scope = "type", Target = "Yasc.AI.RandomAiController")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ai", Scope = "type", Target = "Yasc.AI.UsiAiController")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Usi", Scope = "type", Target = "Yasc.AI.UsiAiController")]
[assembly: SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Scope = "type", Target = "Yasc.AI.UsiAiController")]
[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Yasc.Gui.ConnectingViewModel.#TryingToConnect(System.Object)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "type", Target = "Yasc.Gui.GameTicket")]
[assembly: SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Scope = "member", Target = "Yasc.Gui.GameTicket.#Dispose()")]
[assembly: SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "member", Target = "Yasc.Gui.GameTicket.#Dispose()")]
[assembly: SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "member", Target = "Yasc.Gui.GameViewModel.#Dispose()")]
[assembly: SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Scope = "member", Target = "Yasc.Gui.GameViewModel.#Dispose()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Yasc.Gui.WelcomeViewModel.#IsServerStartedOnThisComputer")]
[assembly: SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "type", Target = "Yasc.Gui.GameViewModel")]
[assembly: SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Scope = "member", Target = "Yasc.Gui.UserViewModel.#InvitationAccepted")]

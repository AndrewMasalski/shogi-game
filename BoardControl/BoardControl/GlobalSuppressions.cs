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

[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Yasc", Scope = "namespace", Target = "Yasc.Common")]
[assembly: SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Yasc.Common.StopwatchControl.#FlipCommand")]
[assembly: SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Yasc.Common.StopwatchControl.#StartCommand")]
[assembly: SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Yasc.Common.StopwatchControl.#StopCommand")]
[assembly: SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Yasc.Common.StopwatchControl.#ResetCommand")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Yasc.Common.StopwatchControl.#.cctor()")]

[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "type", Target = "Yasc.Controls.ShogiPiece")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Yasc", Scope = "namespace", Target = "Yasc.Controls")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Yasc", Scope = "namespace", Target = "Yasc.Controls.Automation")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "type", Target = "Yasc.Controls.ShogiBoard")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "type", Target = "Yasc.Controls.Automation.ShogiBoardAutomationPeer")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "type", Target = "Yasc.Controls.ShogiBoardCore")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "type", Target = "Yasc.Controls.Automation.ShogiBoardCoreAutomationPeer")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "type", Target = "Yasc.Controls.ShogiCell")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "type", Target = "Yasc.Controls.Automation.ShogiCellAutomationPeer")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "type", Target = "Yasc.Controls.ShogiHand")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "type", Target = "Yasc.Controls.Automation.ShogiHandAutomationPeer")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "type", Target = "Yasc.Controls.Automation.ShogiPieceAutomationPeer")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "member", Target = "Yasc.Controls.PieceHolderBase.#ShogiPiece")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "member", Target = "Yasc.Controls.ShogiBoardCore.#ShogiCells")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Yasc.Controls.HandNest.#.cctor()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Yasc.Controls.BoardBorder.#.cctor()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Yasc.Controls.PieceHolderBase.#.cctor()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Yasc.Controls.ShogiBoard.#.cctor()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Yasc.Controls.ShogiBoardCore.#.cctor()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Yasc.Controls.ShogiHand.#.cctor()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Yasc.Controls.ShogiPiece.#.cctor()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Yasc.Controls.SwitchControl.#.cctor()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Yasc.Controls.ShogiCell.#.cctor()")]


[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "shogi", Scope = "member", Target = "Yasc.GenericDragDrop.DragFromHandEventArgs.#.ctor(Yasc.Controls.ShogiBoard,Yasc.Controls.ShogiPiece)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Yasc", Scope = "namespace", Target = "Yasc.GenericDragDrop")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "type", Target = "Yasc.GenericDragDrop.VisualTreeUtils")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dnd", Scope = "type", Target = "Yasc.GenericDragDrop.DndAdorner")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "member", Target = "Yasc.GenericDragDrop.DragFromBoardEventArgs.#FromShogiCell")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "member", Target = "Yasc.GenericDragDrop.DragFromBoardEventArgs.#ShogiPiece")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "member", Target = "Yasc.GenericDragDrop.DragFromHandEventArgs.#ShogiBoard")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "member", Target = "Yasc.GenericDragDrop.DragFromHandEventArgs.#ShogiHand")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "member", Target = "Yasc.GenericDragDrop.DragFromHandEventArgs.#ShogiPiece")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shogi", Scope = "member", Target = "Yasc.GenericDragDrop.DropToBoardEventArgs.#ToShogiCell")]



[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Behaviour", Scope = "type", Target = "Yasc.Common.ListBoxFocusBehaviour")]

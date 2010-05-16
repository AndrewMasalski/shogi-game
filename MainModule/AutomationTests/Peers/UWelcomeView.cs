using BoardControl.AutomationTests.Peers;
using MainModule.Gui;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;

namespace MainModule.AutomationTests.Peers
{
  public class UWelcomeView : WpfCustom
  {
    private WpfButton _playWithCompButton;
    private WpfButton _playWithMyselfButton;
    private WpfButton _connectButton;
    private WpfEdit _nameEdit;
    private WpfComboBox _serverCombo;
    private WpfText _letMeBeAServerLink;
    private WpfCheckBox _dontShowThisPaheAgainCheckBox;

    public UWelcomeView(UITestControl parent)
      : base(parent)
    {
      SearchProperties[PropertyNames.ClassName] = typeof (WelcomeView).UiaClassName();
    }
    public WpfButton PlayWithCompButton
    {
      get
      {
        if (_playWithCompButton == null)
        {
          _playWithCompButton = new WpfButton(this);
          _playWithCompButton.SearchProperties[PropertyNames.Name] = "Play with comp.";
        }
        return _playWithCompButton;
      }
    }
    public WpfButton PlayWithMyselfButton
    {
      get
      {
        if (_playWithMyselfButton == null)
        {
          _playWithMyselfButton = new WpfButton(this);
          _playWithMyselfButton.SearchProperties[PropertyNames.Name] = "Play with myself";
        }
        return _playWithMyselfButton;
      }
    }
    public WpfButton ConnectButton
    {
      get
      {
        if (_connectButton == null)
        {
          _connectButton = new WpfButton(this);
          _connectButton.SearchProperties[PropertyNames.Name] = "Connect";
        }
        return _connectButton;
      }
    }
    public WpfText LetMeBeAServerLink
    {
      get
      {
        if (_letMeBeAServerLink == null)
        {
          _letMeBeAServerLink = new WpfText(this);
          _letMeBeAServerLink.SearchProperties[PropertyNames.Name] = "Let me be a server";
        }
        return _letMeBeAServerLink;
      }
    }
    public WpfCheckBox DontShowThisPaheAgainCheckBox
    {
      get
      {
        if (_dontShowThisPaheAgainCheckBox == null)
        {
          _dontShowThisPaheAgainCheckBox = new WpfCheckBox(this);
          _dontShowThisPaheAgainCheckBox.SearchProperties[PropertyNames.Name] = "Don't show this page again";
        }
        return _dontShowThisPaheAgainCheckBox;
      }
    }
    public bool IsNameValid
    {
      get { return string.IsNullOrEmpty(NameEdit.HelpText); }
    }
    public bool IsServerValid
    {
      get { return string.IsNullOrEmpty(ServerCombo.HelpText); }
    }
    public WpfEdit NameEdit
    {
      get
      {
        if (_nameEdit == null)
        {
          _nameEdit = new WpfEdit(this);
          _nameEdit.SearchProperties[PropertyNames.AutomationId] = "_nameEdit";
        }
        return _nameEdit;
      }
    }
    public WpfComboBox ServerCombo
    {
      get
      {
        if (_serverCombo == null)
        {
          _serverCombo = new WpfComboBox(this);
          _serverCombo.SearchProperties[PropertyNames.AutomationId] = "_serverCombo";
        }
        return _serverCombo;
      }
    }
  }
}
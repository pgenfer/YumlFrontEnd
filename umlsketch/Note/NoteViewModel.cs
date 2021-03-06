﻿using Caliburn.Micro;
using UmlSketch.Command;
using UmlSketch.DomainObject;

namespace UmlSketch.Editor
{
    public class NoteViewModel : PropertyChangedBase
    {
        private readonly ChangeNoteTextCommand _changeNoteTextCommand;
        private string _text;
        private readonly BackgroundColorMixin _backgroundColor;
        private readonly ExpandableMixin _expandable = new ExpandableMixin();

        public NoteViewModel(
            IChangeColorCommand changeNoteColorCommand,
            ChangeNoteTextCommand changeNoteTextCommand,
            DiagramColorPalette diagramColorPalette)
        {
            _changeNoteTextCommand = changeNoteTextCommand;
            _backgroundColor = new BackgroundColorMixin(changeNoteColorCommand, diagramColorPalette);
            _backgroundColor.PropertyChanged += (s, e) => NotifyOfPropertyChange(e.PropertyName);
            _expandable.PropertyChanged += (s, e) => NotifyOfPropertyChange(e.PropertyName);
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _changeNoteTextCommand.ChangeText(value);
                NotifyOfPropertyChange();
            }
        }

        public System.Windows.Media.Color InitialColor
        {
            get { return _backgroundColor.InitialColor; }
            set { _backgroundColor.InitialColor = value; }
        }

        public System.Windows.Media.Color BackgroundColor
        {
            get { return _backgroundColor.BackgroundColor; }
            set { _backgroundColor.BackgroundColor = value; }
        }

        public bool IsExpanded
        {
            get { return _expandable.IsExpanded; }
            set { _expandable.IsExpanded = value; }
        }

        public void ExpandOrCollapse() => _expandable.ExpandOrCollapse();

        public void Delete()
        {
            Text = string.Empty;
            IsExpanded = false;
        }

        public DiagramColorPalette ColorPalette => _backgroundColor.ColorPalette;
    }
}

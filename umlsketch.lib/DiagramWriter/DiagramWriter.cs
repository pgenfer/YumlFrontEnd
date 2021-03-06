﻿using UmlSketch.DomainObject;

namespace UmlSketch.DiagramWriter
{

    /// <summary>
    /// root class used for drawing diagrams.
    /// The class uses a DiagramContentMixin for
    /// writing the diagram content. 
    /// Clients can access the more specific interface of this class
    /// for drawing.
    /// This architecture is organized as a bridge:
    /// The (*)Writer classes are the abstractions that 
    /// can be called by the client while the 
    /// DiagramContentMixin is the concrete implementation
    /// that is used by the writer classes.
    /// </summary>
    public class DiagramWriter
    {
        private readonly DiagramContentMixin _content;

        public ClassWriter StartClass()
        {
            _content.AppendToken("[");
            return new ClassWriter(false, _content);
        }

        public DiagramWriter WithNote(Note note)
        {
            _content.AppendToken($"[note: {note.Text}");
            if(note.HasColor)
                _content.AppendToken($"{{bg:{note.Color}}}");
            _content.AppendToken("]");
            return this;
        }

        public DiagramWriter WithClassifierNote(string classifier, Note note)
        {
            _content.AppendToken($"[{classifier}]-");
            return WithNote(note);
        }

        public RelationWriter StartRelation() => new RelationWriter(_content, string.Empty,true);

        public override string ToString() => _content.ToString();
        public DiagramWriter(DiagramContentMixin content = null)
        {
            _content = content ?? new DiagramContentMixin();
        }

        public DiagramWriter AddSeparator()
        {
            _content.AppendToken(",");
            return this;
        }
    }
}


using System.Collections.Generic;
using System.Numerics;

public class Transform
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; } = Vector3.One;

    private Transform _parent;
    private readonly List<Transform> _children = new();

    public Transform Parent
    {
        get => _parent;
        set
        {
            if (_parent != null)
            {
                _parent._children.Remove(this);
            }

            _parent = value;

            if (_parent != null)
            {
                _parent._children.Add(this);
            }
        }
    }

    public IReadOnlyList<Transform> Children => _children.AsReadOnly();

    public Matrix4x4 LocalToWorldMatrix
    {
        get
        {
            var localMatrix = Matrix4x4.CreateScale(Scale) *
                              Matrix4x4.CreateFromQuaternion(Rotation) *
                              Matrix4x4.CreateTranslation(Position);

            if (Parent != null)
            {
                return localMatrix * Parent.LocalToWorldMatrix;
            }

            return localMatrix;
        }
    }

    public Matrix4x4 WorldToLocalMatrix
    {
        get
        {
            Matrix4x4.Invert(LocalToWorldMatrix, out var result);
            return result;
        }
    }

    public Vector3 TransformPoint(Vector3 point)
    {
        return Vector3.Transform(point, LocalToWorldMatrix);
    }

    public Vector3 InverseTransformPoint(Vector3 point)
    {
        return Vector3.Transform(point, WorldToLocalMatrix);
    }
}

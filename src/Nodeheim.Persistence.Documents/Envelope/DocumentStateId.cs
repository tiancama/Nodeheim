using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents the computation of the state ID of a document.
/// </summary>
/// <remarks>
/// Format contract: The state ID is computed over the document tree without the child <c>stateId</c> of the
/// root entry. The tree is traversed depth-first from the root entry, each entry before its children, with
/// children ordered ordinally by name and in their order within the same name. Each entry contributes its
/// name, a type byte (0x00 for a leaf, 0x01 for a branch), and then either its text or the number of its
/// children followed by the children. Texts and names are encoded as UTF-8 and preceded by their length in
/// bytes; lengths and numbers are 4-byte unsigned big-endian integers. The state ID consists of the first 16
/// bytes of the SHA-256 hash of this sequence, written as lowercase hexadecimal.
/// </remarks>
internal static class DocumentStateId
{
    private const byte LeafType = 0x00;
    private const byte BranchType = 0x01;
    private const int StateIdLength = 16;

    private static readonly UTF8Encoding StrictUtf8 =
        new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    internal static string Compute(DocumentEntry root)
    {
        ArgumentNullException.ThrowIfNull(root);

        using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        AppendEntry(hash, root, isRoot: true);

        Span<byte> digest = stackalloc byte[SHA256.HashSizeInBytes];
        hash.GetHashAndReset(digest);
        return Convert.ToHexStringLower(digest[..StateIdLength]);
    }

    private static void AppendEntry(IncrementalHash hash, DocumentEntry entry, bool isRoot)
    {
        AppendText(hash, entry.Name);

        if (entry.Children.Count == 0)
        {
            AppendByte(hash, LeafType);
            AppendText(hash, entry.Text);
            return;
        }

        IEnumerable<DocumentEntry> children = entry.Children;
        if (isRoot)
        {
            children = children.Where(child => child.Name != DocumentMapper.StateIdName);
        }

        List<DocumentEntry> ordered = children.OrderBy(child => child.Name, StringComparer.Ordinal).ToList();

        AppendByte(hash, BranchType);
        AppendNumber(hash, ordered.Count);
        foreach (DocumentEntry child in ordered)
        {
            AppendEntry(hash, child, isRoot: false);
        }
    }

    private static void AppendText(IncrementalHash hash, string text)
    {
        byte[] bytes = StrictUtf8.GetBytes(text);
        AppendNumber(hash, bytes.Length);
        hash.AppendData(bytes);
    }

    private static void AppendNumber(IncrementalHash hash, int value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        BinaryPrimitives.WriteUInt32BigEndian(buffer, (uint)value);
        hash.AppendData(buffer);
    }

    private static void AppendByte(IncrementalHash hash, byte value)
    {
        Span<byte> buffer = stackalloc byte[1];
        buffer[0] = value;
        hash.AppendData(buffer);
    }
}

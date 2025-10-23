namespace University.Domain.Aggregates.Theses;

public sealed class ThesisAttachment
{
	public ThesisAttachment(string fileName, string contentType, string blobName, long sizeBytes)
		: this(Guid.NewGuid(), fileName, contentType, blobName, sizeBytes)
	{
	}

	private ThesisAttachment()
	{
		FileName = string.Empty;
		ContentType = string.Empty;
		BlobName = string.Empty;
	}

	private ThesisAttachment(Guid id, string fileName, string contentType, string blobName, long sizeBytes)
	{
		if (string.IsNullOrWhiteSpace(fileName))
		{
			throw new ArgumentException("File name must be provided", nameof(fileName));
		}

		if (string.IsNullOrWhiteSpace(contentType))
		{
			throw new ArgumentException("Content type must be provided", nameof(contentType));
		}

		if (string.IsNullOrWhiteSpace(blobName))
		{
			throw new ArgumentException("Blob name must be provided", nameof(blobName));
		}

		if (sizeBytes < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(sizeBytes));
		}

		Id = id;
		FileName = fileName;
		ContentType = contentType;
		BlobName = blobName;
		SizeBytes = sizeBytes;
	}

	public Guid Id { get; private set; }

	public string FileName { get; private set; }

	public string ContentType { get; private set; }

	public string BlobName { get; private set; }

	public long SizeBytes { get; private set; }
}

using Courses.Domain.Shared.Primitives;

namespace Courses.Domain.MediaPackages.Primitives;

public sealed record RawAsset(Url FileUrl, RawAssetType Type = RawAssetType.Unknown);

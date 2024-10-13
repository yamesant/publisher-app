﻿namespace PublisherDomain;

public class Artist
{
    public int ArtistId { get; set; }
    public PersonName Name { get; set; }
    public List<Cover> Covers { get; set; } = new();
}

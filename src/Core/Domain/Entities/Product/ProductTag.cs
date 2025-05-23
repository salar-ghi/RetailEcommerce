﻿namespace Domain.Entities;

public class ProductTag : BaseModel<int>
{
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}

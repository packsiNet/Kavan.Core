namespace ApplicationLayer.Dto.External.Dune;

public class DuneQueryResultDto<T>
{
    public string execution_id { get; set; } = string.Empty;
    public int query_id { get; set; }
    public bool is_execution_finished { get; set; }
    public string state { get; set; } = string.Empty;
    public DateTime submitted_at { get; set; }
    public DateTime expires_at { get; set; }
    public DateTime execution_started_at { get; set; }
    public DateTime execution_ended_at { get; set; }

    public DuneResultBody<T> result { get; set; } = new();
}

public class DuneResultBody<T>
{
    public List<T> rows { get; set; } = new();
    public DuneResultMetadata metadata { get; set; } = new();
}

public class DuneResultMetadata
{
    public string[] column_names { get; set; } = Array.Empty<string>();
    public string[] column_types { get; set; } = Array.Empty<string>();
    public int row_count { get; set; }
    public int result_set_bytes { get; set; }
    public int total_row_count { get; set; }
    public int total_result_set_bytes { get; set; }
    public int datapoint_count { get; set; }
    public int pending_time_millis { get; set; }
    public int execution_time_millis { get; set; }
}

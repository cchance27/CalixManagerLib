namespace CalixManager.Models.Postgrest;

public class Ont
{
    public int networkref { get; init; }
    public string? addressid { get; init; }
    public int aidtype {get; init;}
    public int straid {get; init;}
    //"ex_pwe3prof_bytes": null,
    //"ex_pwe3prof_tlvtype": null,
    //"ex_pwe3prof_straid": null,
    //"ex_linkedpon_bytes": "\\000\\000\\000\\001\\000\\000\\000\\001\\000\\000\\000\\007",
    //"ex_linkedpon_tlvtype": 15131,
    public string? ex_linkedpon_straid { get; init; }
    //"ex_ontprof_bytes": null,
    //"ex_ontprof_tlvtype": 15127,
    //"ex_ontprof_straid": "157(844G)",
    public string? serno { get; init; }
    public string? descr { get; init; }
    public string? subscr_id { get; init; }
    //"admin": 1,
    //"op_stat": 1,
    //"reg_id": null,
    //"low_rx_opt_pwr_ne_thresh": "-30.00",
    //"high_rx_opt_pwr_ne_thresh": "-7.00",
    public string? model { get; init; }
    public byte? ds_sdber_rate { get; init; }
    public byte? us_sdber_rate { get; init; }
    public byte? cur_ds_sdber_rate { get; init; }
    public byte? cur_us_sdber_rate { get; init; }
    //"battery_present": 1,
    //"pse_max_power_budget": null,
    public string? vendor { get; init; }
    //"clei": "BVMCH00ARE",
    
    //"product_code": "P0",
    //"tx_opt_lvl": "2.000",

    //"curr_cust_vers": null,
    //"alt_cust_vers": null,
    public float? fe_opt_lvl { get; init; }
    public float? opt_sig_lvl { get; init; }
    public int? range_length { get; init; }
    public double? uptime { get; init; }
    public string? curr_sw_vers { get; init;}
    public string? alt_sw_vers { get; init; }
    public string? curr_committed { get; init; }
    public string? onu_mac   {get; init;}
    public string? mta_mac   {get; init;}
    public string? mfg_serno { get; init; }
    //"rg_config_file_vers": "0             ",
    //"voip_config_file_vers": "0             ",
}
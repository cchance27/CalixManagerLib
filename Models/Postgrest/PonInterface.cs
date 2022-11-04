namespace CalixManager.Models.Postgrest;

public class PonInterface
{
    public int networkref { get; init; }
    public string? addressid { get; init; }
    public int aidtype { get; init; }
    public string? straid { get; init; }
    public string? card { get; init; }
    public float? sfp_temp { get; init; }
    public float? sfp_tx_bias { get; init; }
    public float? sfp_tx_power { get; init; }
    public float? sfp_rx_power { get; init; }
    public float? sfp_voltage { get; init; }
    public int? sfp_line_length { get; init; }
    public float? sfp_wavelength { get; init; }
    public byte admin { get; init; }
    public byte op_stat { get; init; }
    public byte status { get; init; }
    public string? descr { get; init; }
    //"rate_limit": null,
    //"max_streams": null,
    //"stream_alarm_lvl": null,
    //"mcast_bw": null,
    //"mcast_bw_alarm_lvl": null,
    //"split_hor": 1,
    //"dyn_bw_alloc": 1,
    //"ds_sdber_rate": 5,
    //"ex_pon_gos_bytes": null,
    //"ex_pon_gos_tlvtype": 9822,
    //"ex_pon_gos_straid": "1",
    //"high_tx_opt_pwr_ne_thresh": "10.00",
    //"low_tx_opt_pwr_ne_thresh": "-16.00",
    //"optical_monitoring": 1,
    //"us_fec": 1,
    //"ds_fec": 1,
    //"aes_enable": 0,
    //"sfp_status": 0,
    //"sfp_type": 14,
    //"sfp_conn": 1,
    //"sfp_encoding": 0,
    //"sfp_bitrate": 24,
    //"sfp_bitratemax": 0,
    //"sfp_vendname": "Calix",
    //"sfp_vendoui": null,
    //"sfp_vendpartno": "100-01782",
    //"sfp_vendrev": null,
    //"sfp_vendserno": "P395A011111",
    //"sfp_calixpartno": null,
    //"v2_joins_sent": 0,
    //"v2_joins_rec": 0,
    //"v3_reports_sent": null,
    //"v3_reports_rec": null,
    //"v3_incl_allow_rec": null,
    //"v3_excl_block_rec": null,
    //"v3_is_incl_sent": 0,
    //"v3_is_incl_rec": 0,
    //"v3_is_excl_sent": 0,
    //"v3_is_excl_rec": 0,
    //"v3_to_incl_sent": 0,
    //"v3_to_incl_rec": 0,
    //"v3_to_excl_sent": 0,
    //"v3_to_excl_rec": 0,
    //"v3_allow_sent": 0,
    //"v3_allow_rec": 0,
    //"v3_block_sent": 0,
    //"v3_block_rec": 0,
    //"leaves_sent": 0,
    //"leaves_rec": 0,
    //"gsq_sent": 0,
    //"gsq_rec": 0,
    //"inval_msg": 0,
    //"total_pon_bw": null,
    //"guaranteed_bw_max": null,
    //"guaranteed_bw_in_use": null,
    //"subscribed_bw_in_use": null,
    //"over_subscribed_bw_in_use": null,
    //"query_solicits_sent": 0,
    //"query_solicits_rec": 0,
    //"general_querys_sent": null,
    //"general_querys_rec": null,
    //"v2_general_querys_sent": 0,
    //"v2_general_querys_rec": 0,
    //"v3_general_querys_sent": 0,
    //"v3_general_querys_rec": 0,
    //"clei": "BVL3AM7FAA",
    //"rogue_info": null,
    //"rogue_ont_detection": "enabled",
    //"ip_src_verify_invalid_count": null
}
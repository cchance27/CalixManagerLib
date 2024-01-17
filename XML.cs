using CalixManager.Models.NetConf;

namespace CalixManager;

public static class XMLMessages
{
        static string CalixEnvelope(string xmlInternal) => 
        $"""
        <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/">
            {xmlInternal}
        </soapenv:Envelope>
        """;

    static string CalixBody(string xmlInternal) => 
        CalixEnvelope(
            $"""
                <soapenv:Body>
                    {xmlInternal}
                </soapenv:Body>
            """);

    static string CalixRpc(MessageData md, string node, string xmlInternal) =>
        CalixBody(
            $"""
                <rpc message-id="{md.messageId}" nodename="{node}" timeout="{md.timeout}" username="{md.username}" sessionid="{md.sessionId}">
                    {xmlInternal}
                </rpc>
            """);

    public static string Login(MessageData md) => 
        CalixBody(
            $"""
            <auth message-id="{md.messageId}">
                <login>
                    <UserName>{md.username}</UserName>
                    <Password>{md.password}</Password>
                </login>
            </auth>
            """);

    public static string Logout(MessageData md) => 
        CalixBody(
            $"""
            <auth message-id="{md.messageId}">
                <logout>
                    <UserName>{md.username}</UserName>
                    <SessionId>{md.sessionId}</SessionId>
                </logout>
            </auth>
            """);

    public static string CreateOnt(MessageData md, string serial, string node, int ontprof = 157) => 
        CalixRpc(md, node, 
            $"""
            <edit-config>
                <target><running/></target>
                <config>
                    <top>
                        <object operation="create" get-config="true">
                        <type>Ont</type>
                        <id>
                            <ont>0</ont>
                        </id>
                        <admin>enabled</admin>
                        <serno>{serial}</serno>
                        <reg-id></reg-id>
                        <subscr-id></subscr-id>
                        <descr></descr>
                        <ontprof>
                            <type>OntProf</type>
                            <id>
                                <ontprof>{ontprof}</ontprof>
                            </id>
                        </ontprof>
                     </object>
                  </top>
               </config>
            </edit-config>
            """);

    public static string GetOnt(MessageData md, string node, string serial) =>
        CalixRpc(md, node,
            $"""
            <action>
                <action-type>show-ont</action-type>
                <action-args>
                     <serno>{serial}</serno>
                </action-args>
            </action>
            """);

    public static string GetOntProfile(MessageData md, string node, int model) =>
        CalixRpc(md, node,
            $"""
            <get-config>
                <source>
                    <running/>
                </source>
                <filter type="subtree">
                    <top>
                        <object>
                            <type>OntProf</type>
                            <id>
                                <ontprof>{model}</ontprof>
                            </id>
                        </object>
                    </top>
                </filter>
            </get-config>
            """);

    public static string GetOntDetail(MessageData md, string node, string id) =>
        CalixRpc(md, node,
            $"""
            <get>
                <filter type="subtree">
                    <top>
                        <object>
                            <type>Ont</type>
                            <id>
                                <ont>{id}</ont>
                            </id>
                        </object>
                    </top>
                </filter>
            </get>
            """);

    public static string GetEthSvc(MessageData md, string node, int id, bool limited = false) =>
        CalixRpc(md, node,
            $"""
            <get-config>
                <source><running/></source>
                <filter type="subtree">
                    <top>
                        <object>
                        <type>Ont</type>
                        <id><ont>{id}</ont></id>
                        <children>
                            <type>EthSvc</type>
                            {(limited ? "<attr-list>admin descr tag-action bw-prof mcast-prof</attr-list>" : "")}
                        </children>
                        </object>
                    </top>
                </filter>
            </get-config>
            """);

    public static string ProvisionRGService(MessageData md, string node, int ontId) =>
        CalixRpc(md, node,
            $"""
            <edit-config>
              <target><running/></target>
              <config>
                  <top>
                      <object operation="create" get-config="true">
                          <type>EthSvc</type>
                          <id>
                              <ont>{ontId}</ont>
                              <ontslot>8</ontslot>
                              <ontethany>1</ontethany>
                              <ethsvc>1</ethsvc>
                          </id>
                          <admin>enabled</admin>
                          <tag-action>
                              <type>SvcTagAction</type>
                              <id>
                                  <svctagaction>2</svctagaction>
                             </id>
                          </tag-action>
                          <bw-prof>
                              <type>BwProf</type>
                              <id>
                                  <bwprof>1</bwprof>
                              </id>
                          </bw-prof>
                      </object>
                   </top>
                </config>
            </edit-config>
            """);

    public static string SetOntRG(MessageData md, string node, int ontId, int ontSlot, int ontRg, string wantype, string pppUser, string pppPass, string staticip, string staticmask, string staticgw, string staticdns, string staticdns2, bool enabled = true) => 
        CalixRpc(md, node,
            $"""
            <edit-config>
                <target><running/></target>
                <config>
                   <top>
                       <object operation="merge">
                           <type>OntRg</type>
                           <id>
                               <ont>{ontId}</ont>
                               <ontslot>{ontSlot}</ontslot>
                               <ontrg>{ontRg}</ontrg>
                           </id>
                           <descr></descr>
                           <subscr-id></subscr-id>
                           <admin>{(enabled ? "enabled" : "disabled")}</admin>
                           <mgmt-mode>native</mgmt-mode>
                           <wan-protocol>{wantype}</wan-protocol>
                           <pppoe-user>{pppUser}</pppoe-user>
                           <pppoe-password>{pppPass}</pppoe-password>
                           <static-ip>{staticip}</static-ip>
                           <static-ip-mask>{staticmask}</static-ip-mask>
                           <static-ip-gw>{staticgw}</static-ip-gw>
                           <pri-dns-server>{staticdns}</pri-dns-server>
                           <sec-dns-server>{staticdns2}</sec-dns-server>
                       </object>
                   </top>
               </config>
            </edit-config>
        """);

    public static string GetResGwState(MessageData md, string node, int ontId, int rg) =>
        CalixRpc(md, node,
            $"""
            <get>
                <filter type="subtree">
                    <top>
                        <object>
                            <type>OntRg</type>
                            <id>
                                <ont>{ontId}</ont>
                                <ontslot>8</ontslot>
                                <ontrg>{rg}</ontrg>
                            </id>
                        </object>
                    </top>
                </filter>
            </get>
            """);

    public static string GetAllOntEthSvcConfig(MessageData md, string node, int ontId) =>
        CalixRpc(md, node,
            $"""
            <get-config>
                <source>
                    <running/>
                </source>
                <filter type="subtree">
                    <top>
                        <object>
                            <type>Ont</type>
                            <id>
                                <ont>{ontId}</ont>
                            </id>
                            <children>
                                <type>EthSvc</type>
                                <attr-list>admin descr tag-action bw-prof out-tag in-tag mcast-prof pon-cos us-cir-override us-pir-override ds-pir-override</attr-list>
                            </children>
                         </object>
                    </top>
                </filter>
            </get-config>
            """);

    public static string GetAllOntRgConfig(MessageData md, string node, int ontId) =>
    CalixRpc(md, node,
        $"""
            <get-config>
                <source>
                    <running/>
                </source>
                <filter type="subtree">
                    <top>
                        <object>
                            <type>Ont</type>
                            <id>
                                <ont>{ontId}</ont>
                            </id>
                            <children>
                                <type>OntRg</type>
                                <attr-list>admin pppoe-user pppoe-password wan-protocol static-ip static-ip-mask static-ip-gw pri-dns-server sec-dns-server set-remote-access-secs</attr-list>
                            </children>
                         </object>
                    </top>
                </filter>
            </get-config>
            """);

        public static string SetRemoteAccessTime(MessageData md, string node, int ontId, int slot, int rg, int seconds) =>
        CalixRpc(md, node,
            $"""
            <edit-config>
                <target>
                    <running/>
                </target>
                <config>
                    <top>
                        <object operation="merge">
                            <type>OntRg</type>
                            <id>
                                <ont>{ontId}</ont>
                                <ontslot>{slot}</ontslot>
                                <ontrg>{rg}</ontrg>
                            </id>
                            <set-remote-access-secs>{seconds}</set-remote-access-secs>
                        </object>
                    </top>
                </config>
            </edit-config>
            """);

        public static string SetONTOwnerDetails(MessageData md, string node, int ontId, string descr, string subscrid) =>
        CalixRpc(md, node,
            $"""
            <edit-config>
                <target>
                    <running/>
                </target>
                <config>
                    <top>
                        <object operation="merge">
                            <type>Ont</type>
                            <id>
                                <ont>{ontId}</ont>
                            </id>
                            <descr>{descr}</descr>
                            <subscr-id>{subscrid}</subscr-id>
                        </object>
                    </top>
                </config>
            </edit-config>
            """);

        public static string GetGponPortConfig(MessageData md, string node, int shelf, int card, int port) => 
        CalixRpc(md, node, 
            $"""
            <get-config>
                    <source><running/></source>
                    <filter type="subtree">
                        <top>
                            <object>
                                <type>GponPort</type>
                                <id>
                                    <shelf>{shelf}</shelf>
                                    <card>{card}</card>
                                    <gponport>{port}</gponport>
                                </id>
                            </object>
                        </top>
                    </filter>
                </get-config>    
            """);
        
        public static string GetGponPortStatus(MessageData md, string node, int shelf, int card, int port) => 
        CalixRpc(md, node, 
            $"""
            <get>
                <filter type="subtree">
                     <top>
                         <object>
                            <type>GponPort</type>
                            <id>
                                <shelf>{shelf}</shelf>
                                <card>{card}</card>
                                <gponport>{port}</gponport>
                            </id>
                            <attr-list>op-stat status sfp-status sfp-type sfp-conn  sfp-temp sfp-tx-bias sfp-tx-power sfp-rx-power sfp-voltage sfp-line-length sfp-wavelength clei</attr-list>
                        </object>
                    </top>
                </filter>
            </get> 
            """);
        
}

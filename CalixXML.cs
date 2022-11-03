﻿namespace Calix;

public static class CalixXML
{
    public struct MessageData
    {
        int id = 0;
        int? session = null;

        public int messageId {
            get {
                id++;
                return id;
            }
        }

        public int sessionId
        {
            get
            {
                ArgumentNullException.ThrowIfNull(session);
                return session.Value;
            }
            set
            {
                if (value > -1) 
                    session = value;
            }
        }

        public string username = "rootgod";
        public string password = "root";
        public int timeout = 35000;

        public MessageData(string username = "rootgod", string password = "root", int timeout = 35000)
        {
            this.username = username;
            this.password = password;
            this.timeout = timeout;
        }
    }

    static string CalixEnvelope(string xmlInternal) => $"""
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

    public static string SetRGToPPPoE(MessageData md, string node, int ontId, string pppUser, string pppPass) => 
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
                               <ontslot>8</ontslot>
                               <ontrg>1</ontrg>
                           </id>
                           <admin>enabled</admin>
                           <mgmt-mode>native</mgmt-mode>
                           <wan-protocol>pppoe</wan-protocol>
                           <pppoe-user>{pppUser}</pppoe-user>
                           <pppoe-password>{pppPass}</pppoe-password>
                       </object>
                   </top>
               </config>
            </edit-config>
        """);
}
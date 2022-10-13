function loadTalkingPoints() {
	var client = Xrm.Page.context.client.getClient();
	var clientState = Xrm.Page.context.client.getClientState();
	if (client == "Outlook" || clientState == "Offline") {
		return;
	}
	else
	{
		var isUci = Xrm.Internal.isUci();

		if (isUci) {
			var isTalkingPointsEnabled = Xrm.Internal.isFeatureEnabled("TalkingPoints");
			if (isTalkingPointsEnabled) {
				var isAdminSettingEnabled = null;
				var isSSSConfigured = null;
				if (sessionStorage) {
					isAdminSettingEnabled = sessionStorage.getItem("icebreakers_isadminsettingenabled");
					isSSSConfigured = sessionStorage.getItem("icebreakers_isSSSConfigured");
				}
				if (isAdminSettingEnabled == null) {
					setIcebreakersConfigSettings();
				}
				else if (isAdminSettingEnabled == "true") {
					if (isSSSConfigured == null)
						setUserMailBoxConfigSettings();
					else if (isSSSConfigured == "true")
						showIcebreakers();
				}
			}
		}
	}
}

function setIcebreakersConfigSettings() {
	var fetchXmlforIcebreakersConfig = "<fetch mapping='logical'><entity name='msdyn_icebreakersconfig'><attribute name='msdyn_icebreakersconfigid'/>"
		+ "<attribute name='msdyn_isadminsettingenabled'/></entity></fetch>";
	Xrm.WebApi.retrieveMultipleRecords("msdyn_icebreakersconfig", "?fetchXml=" + fetchXmlforIcebreakersConfig).then(function (response) {
		var isEnabled = false;
		if (response != null && response.entities.length > 0) {
			isEnabled = response.entities[0].msdyn_isadminsettingenabled;
			if (isEnabled) {
				setUserMailBoxConfigSettings();
			}
		}
		if (sessionStorage) {
			sessionStorage.setItem("icebreakers_isadminsettingenabled", isEnabled);
		}
	},function (error) {
		return null;
	});
}

function setUserMailBoxConfigSettings() {
	var userId = Xrm.Page.context.getUserId();
	var fetchXmlforMailBoxConfig = "<fetch mapping='logical' version='1.0'><entity name='emailserverprofile'><attribute name='servertype' /><link-entity name='mailbox' from='emailserverprofile' to='emailserverprofileid' alias='mb' link-type='inner'><filter type='and'><condition attribute='incomingemaildeliverymethod' operator='eq' value='2' /><condition attribute='isemailaddressapprovedbyo365admin' operator='eq' value='1' /></filter><link-entity name='systemuser' from='defaultmailbox' to='mailboxid' alias='su' link-type='inner'><filter type='and'><condition attribute='systemuserid' operator='eq' value='" + userId + "'/></filter></link-entity></link-entity></entity></fetch>";
	Xrm.WebApi.retrieveMultipleRecords("emailserverprofile", "?fetchXml=" + fetchXmlforMailBoxConfig).then(function (response) {
		var isSSSConfigured = false;
		if (response != null && response.entities.length > 0) {
			if (response.entities[0].servertype == "0") {
				isSSSConfigured = true;
				showIcebreakers();
			}
		}
		if (sessionStorage) {
			sessionStorage.setItem("icebreakers_isSSSConfigured", isSSSConfigured);
		}
	}, function (error) {
		return null;
	});
}

function showIcebreakers() {
	/* 
	 * Bug 915902 Exception while loading contact form due to talking point control, 
	 * if some contact fails to load talking points
	 * Fix: Added SetTimeout as 0 delay
	 */
	try{
		setTimeout(function () {
			var control = Xrm.Page.getControl("TalkingPoints");
			if (control) {
				control.getParent().setVisible(true);
			}
		}, 0);
	}
	catch(err)
	{
	}
}


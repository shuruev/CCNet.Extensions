<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:template name="defaultStyle">
		<style>
			* { font-family:Calibri, sans-serif; padding:0; margin:0; }
			body { font-size:11pt; background-color:white; color:black; }
			p { margin:0; line-height:100%; }
			table { width:100%;border-collapse:collapse; }
			table, table tr, table td { margin:0; padding:0; border:none; font-size:10pt }
			table td, .change { white-space:nowrap; }
			hr { margin:0; }
			.user .name { font-size:18pt; font-weight:bold; }
			.user .description { font-style:italic; }
			.user.active .name { color:rgb(54, 95, 145); }
			.user.active .description { color:rgb(128, 128, 128); }
			.user.passive .name { color:red; }
			.user.passive .description { color:red; }
			.filename { font-family:monospace; }
			.fileage { color:rgb(128, 128, 128); }
			.spacer { font-size:0.001pt; }
		</style>
    </xsl:template>
</xsl:stylesheet>

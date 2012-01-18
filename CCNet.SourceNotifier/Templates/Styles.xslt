<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:template name="defaultStyle">
		<style>
			* { font-family:Calibri, sans-serif; }
			body { font-size:11pt; background-color:white; color:black; }
			p { margin:0em 0 10pt 0; line-height:115%; }
			table { width:100%;border-collapse:collapse;font-size:0.9091em; }
			table, table tr, table td { margin:0; padding:0; border:none; }
			table td { white-space:nowrap; }
			.user .name { font-size:1.6364em; font-weight:bold; margin:24pt 0 0 0; }
			.user .description { font-style:italic; }
			.user.active .name { color:rgb(54, 95, 145); }
			.user.active .description { color:rgb(128, 128, 128); }
			.user.passive .name { color:red; }
			.user.passive .description { color:red; }
			.filename { font-family:monospace; }
			.fileage { color:rgb(128, 128, 128); }
			.change { white-space:nowrap; }
		</style>
    </xsl:template>
</xsl:stylesheet>

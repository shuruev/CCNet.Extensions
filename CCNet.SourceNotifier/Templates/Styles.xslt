<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:template name="defaultStyle">
		<style>
			* { font-family:Calibri, sans-serif; }
			body { font-size:11pt; }
			p { margin:0.2em 0 0.2em 0; }
			.user .name { font-size:1.6em; font-weight:bold; }
			.user .description { font-style:italic; }
			.user .name, .user .description { margin:0 }
			.user.active .name { color:rgb(79, 129, 189); }
			.user.active .description { color:rgb(128, 128, 128); }
			.user.passive .name { color:red; }
			.user.passive .description { color:red; }
			.filename { font-family:monospace; font-size:0.9em; }
			.fileage { color:rgb(128, 128, 128); font-size:0.9em; }
			ul.changelist li { list-style-type:none; }
			.change { white-space:nowrap; }
			.user { margin-bottom:0.5em; }
			hr.groupseparator { margin:1em 0 1em 0; }
		</style>
    </xsl:template>
</xsl:stylesheet>

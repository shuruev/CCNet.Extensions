<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:template name="defaultStyle">
		<style>
			* { font-family:Calibri, sans-serif; }
			body { font-size:11pt; }
			.user .name { color:rgb(79, 129, 189); font-size:1.6em; margin:0; font-weight:bold; }
			.user .description { color:rgb(128, 128, 128); margin:0; font-style:italic; }
			.user.passive .name { color:red; }
			.user.passive .description { color:red; }
			.filename { font-family:monospace; font-size:0.9em; }
			.fileage { color:rgb(128, 128, 128); font-size:0.9em; }
			.filename, .fileage { margin:0 1em 0 1em; }
			ul.changelist li { list-style-type:none; }
			.change { white-space:nowrap; }
			p { margin:0.2em }
			hr.groupseparator { margin:1em; }
		</style>
    </xsl:template>
</xsl:stylesheet>

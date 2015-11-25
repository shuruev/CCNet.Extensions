<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

	<xsl:output method="xml" indent="yes"/>

	<xsl:template match="CoverageDSPriv">
		<CoverageData>
			<xsl:apply-templates select="Module"/>
		</CoverageData>
	</xsl:template>

	<xsl:template match="Lines"/>
	<xsl:template match="NamespaceKeyName"/>
	<xsl:template match="ClassKeyName"/>
	<xsl:template match="MethodKeyName"/>
	<xsl:template match="MethodFullName"/>

	<xsl:template match="LinesCovered"/>
	<xsl:template match="LinesPartiallyCovered"/>
	<xsl:template match="LinesNotCovered"/>

	<xsl:template match="ImageLinkTime"/>
	<xsl:template match="ImageSize"/>

	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()"/>
		</xsl:copy>
	</xsl:template>

</xsl:stylesheet>

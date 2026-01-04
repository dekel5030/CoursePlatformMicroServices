<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=false; section>
    <#if section = "header">
        <div class="cp-header">
            <h1 class="cp-title">CoursePlatform</h1>
            <#if message?has_content>
                <p class="cp-subtitle">${message.summary}</p>
            <#else>
                <p class="cp-subtitle">${msg("errorTitle")}</p>
            </#if>
        </div>
    <#elseif section = "form">
        <div id="kc-error-message">
            <#if message?has_content && message.type = 'error'>
                <div class="alert alert-error" role="alert">
                    <span class="pficon pficon-error-circle-o"></span>
                    <span class="kc-feedback-text">${kcSanitize(message.summary)?no_esc}</span>
                </div>
            </#if>
            
            <#if client?? && client.baseUrl?has_content>
                <p id="instruction1" class="instruction">
                    ${kcSanitize(msg("errorReturnMessage"))?no_esc}
                </p>
                <div id="kc-form-buttons" class="${properties.kcFormButtonsClass!}">
                    <a href="${client.baseUrl}" class="${properties.kcButtonClass!} ${properties.kcButtonPrimaryClass!} ${properties.kcButtonBlockClass!} ${properties.kcButtonLargeClass!}">${kcSanitize(msg("backToApplication"))?no_esc}</a>
                </div>
            <#else>
                <p id="instruction1" class="instruction">
                    ${kcSanitize(msg("errorMessage"))?no_esc}
                </p>
                <div id="kc-form-buttons" class="${properties.kcFormButtonsClass!}">
                    <a href="${url.loginUrl}" class="${properties.kcButtonClass!} ${properties.kcButtonPrimaryClass!} ${properties.kcButtonBlockClass!} ${properties.kcButtonLargeClass!}">${kcSanitize(msg("backToLogin"))?no_esc}</a>
                </div>
            </#if>
        </div>
    </#if>
</@layout.registrationLayout>

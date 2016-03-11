#Wildcard module for Sitecore
<b>Working with wildcard items with a few simple settings</b>

## Multisite configuration
<p>
In the Sitecore.SharedSource.Wildcard.config you will find the wildcard where you are able to set the wildcard setting folder path for different websites. Just give the name of the setting the same name as the site definition name and set a relative path.
</p>
<code>
    &lt;wildcard&gt;<br />
      &lt;settings&gt;<br />
        &lt;setting name=&quot;Default&quot; path=&quot;/settings/wildcard&quot; /&gt;<br />
      &lt;/settings&gt;<br />
    &lt;/wildcard&gt;
</code>

## Create the datasource folder
<p>Just create a folder or bucket where you want to store pages which could be shared among mutiple websites.</p>

## Create and configure the wildcard setting
<p>Create an item named * within your website(s) based on the template 
/sitecore/templates/modules/wildcard/wildcard. Set the field datasource of 
the * item to the root of your datasource folder or bucket.</p>
<sub>
Example /sitecore/content/Website/Home/News/* points to /sitecore/content/Website/Shared/News
</sub>

## Create and configure the wildcard datasource setting
<p>Create a new wildcard datasource setting item in the folder which is specified in the config file. Set the field item wildcard to the created * item and the field template to the template which the folder or bucket contains.</p>


## How to code











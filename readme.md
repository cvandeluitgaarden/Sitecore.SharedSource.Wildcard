#Wildcard module for Sitecore
<b>Working with wildcard items with a few simple settings</b>

## Multisite configuration
<p>
In the Sitecore.SharedSource.Wildcard.config you will find the wildcard where you are able to set the wildcard setting folder path for different websites. Just give the name of the setting the same name as the site definition name and set a relative path.
</p>

<sub>Sitecore.SharedSource.Wildcard.config</sub>

<code>
    &lt;wildcard&gt;<br />
      &lt;settings&gt;<br />
        &lt;setting name=&quot;Default&quot; path=&quot;/settings/wildcard&quot; /&gt;<br />
      &lt;/settings&gt;<br />
    &lt;/wildcard&gt;
</code>


<span>Figure 1</span>
![Sitecore tree](https://raw.githubusercontent.com/cvandeluitgaarden/Sitecore.SharedSource.Wildcard/master/resources/readme/sitecore_tree.png)

<span>Figure 2</span>
![Sitecore wildcard setting](https://raw.githubusercontent.com/cvandeluitgaarden/Sitecore.SharedSource.Wildcard/master/resources/readme/sitecore_wildcard.png)

<span>Figure 3</span>
![Sitecore tree](https://raw.githubusercontent.com/cvandeluitgaarden/Sitecore.SharedSource.Wildcard/master/resources/readme/sitecore_wildcard_setting.png)

## Create the datasource folder
<p>Create a folder or bucket to save pages which you want to share among mutiple websites.</p><p>Example: as you van see in figure 1 there is a bucket folder /sitecore/content/shared/news in which you store the news items.</p>

## Create and configure the wildcard
<p>Create an item named * within your website(s) based on the template 
/sitecore/templates/modules/wildcard/wildcard. Set the field datasource of 
the * item to the root of your datasource folder or bucket.</p>
<p>Example: as you can see in figure 1 i created a wildcard setting (/sitecore/content/website/settings/wildcard/news). In figure 2 you can see i pointed the datasource field to the datasource folder.</p>


## Create and configure the wildcard datasource setting
<p>Create a new wildcard datasource setting item based on template /sitecore/templates/System/Modules/Wildcard/WildcardSetting in the folder which is specified in the config file. Set the field item wildcard to the created * item and the field template to the template which the folder or bucket contains.</p>
<p>Example: in figure 1 i created a setting in /sitecore/content/Website/Settings/Wildcard/News. In figure 3 you can see i pointed the field Item to the wildcard item and the field DatasourceTemplate to the template of the item which should be placed on the wildcard.</p>










.class public Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;
.super Landroid/preference/PreferenceActivity;
.source "SettingsActivity.java"

# interfaces
.implements Lcom/linkedin/android/litrackinglib/pages/IDisplayKeyProvider;


# annotations
.annotation system Ldalvik/annotation/MemberClasses;
    value = {
        Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$SettingKeys;
    }
.end annotation


# static fields
.field private static final PRIVACY_INNER_PREF_KEY:Ljava/lang/String; = "privacy_inner"

.field private static final TAG:Ljava/lang/String;


# instance fields
.field private _presenter:Lcom/linkedin/android/jobs/jobseeker/presenter/LogOutEventPresenter;

.field private _subscription:Lrx/Subscription;


# direct methods
.method static constructor <clinit>()V
    .locals 1

    .prologue
    .line 50
    const-class v0, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;

    invoke-virtual {v0}, Ljava/lang/Class;->getSimpleName()Ljava/lang/String;

    move-result-object v0

    sput-object v0, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->TAG:Ljava/lang/String;

    return-void
.end method

.method public constructor <init>()V
    .locals 0

    .prologue
    .line 48
    invoke-direct {p0}, Landroid/preference/PreferenceActivity;-><init>()V

    .line 53
    return-void
.end method

.method static synthetic access$000()Ljava/lang/String;
    .locals 1

    .prologue
    .line 48
    sget-object v0, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->TAG:Ljava/lang/String;

    return-object v0
.end method

.method static synthetic access$100(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;Z)Z
    .locals 1
    .param p0, "x0"    # Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;
    .param p1, "x1"    # Z

    .prologue
    .line 48
    invoke-direct {p0, p1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->persistShakeForFeedbackPreference(Z)Z

    move-result v0

    return v0
.end method

.method private getPrefName(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$SettingKeys;)Ljava/lang/String;
    .locals 3
    .param p1, "key"    # Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$SettingKeys;

    .prologue
    .line 318
    const/4 v0, 0x0

    .line 319
    .local v0, "keyString":Ljava/lang/String;
    if-eqz p1, :cond_0

    .line 320
    invoke-virtual {p1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$SettingKeys;->name()Ljava/lang/String;

    move-result-object v1

    sget-object v2, Ljava/util/Locale;->US:Ljava/util/Locale;

    invoke-virtual {v1, v2}, Ljava/lang/String;->toLowerCase(Ljava/util/Locale;)Ljava/lang/String;

    move-result-object v0

    .line 322
    :cond_0
    return-object v0
.end method

.method private static getResIdFromAttribute(Landroid/app/Activity;I)I
    .locals 3
    .param p0, "activity"    # Landroid/app/Activity;
    .param p1, "attr"    # I

    .prologue
    .line 353
    if-nez p1, :cond_0

    .line 354
    const/4 v1, 0x0

    .line 358
    :goto_0
    return v1

    .line 356
    :cond_0
    new-instance v0, Landroid/util/TypedValue;

    invoke-direct {v0}, Landroid/util/TypedValue;-><init>()V

    .line 357
    .local v0, "typedvalueattr":Landroid/util/TypedValue;
    invoke-virtual {p0}, Landroid/app/Activity;->getTheme()Landroid/content/res/Resources$Theme;

    move-result-object v1

    const/4 v2, 0x1

    invoke-virtual {v1, p1, v0, v2}, Landroid/content/res/Resources$Theme;->resolveAttribute(ILandroid/util/TypedValue;Z)Z

    .line 358
    iget v1, v0, Landroid/util/TypedValue;->resourceId:I

    goto :goto_0
.end method

.method private persistShakeForFeedbackPreference(Z)Z
    .locals 1
    .param p1, "value"    # Z

    .prologue
    .line 296
    const-string v0, "pref_shake_for_feedback"

    invoke-static {v0, p1}, Lcom/linkedin/android/jobs/jobseeker/util/SettingPrefUtils;->putBoolean(Ljava/lang/String;Z)V

    .line 297
    const/4 v0, 0x1

    return v0
.end method

.method private removeInternalSection()V
    .locals 4

    .prologue
    .line 309
    :try_start_0
    invoke-virtual {p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v2

    const-string v3, "internal_header"

    invoke-virtual {v2, v3}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v1

    check-cast v1, Landroid/preference/PreferenceCategory;

    .line 311
    .local v1, "internal":Landroid/preference/PreferenceCategory;
    invoke-virtual {p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceScreen()Landroid/preference/PreferenceScreen;

    move-result-object v2

    invoke-virtual {v2, v1}, Landroid/preference/PreferenceScreen;->removePreference(Landroid/preference/Preference;)Z
    :try_end_0
    .catch Ljava/lang/Exception; {:try_start_0 .. :try_end_0} :catch_0

    .line 315
    .end local v1    # "internal":Landroid/preference/PreferenceCategory;
    :goto_0
    return-void

    .line 312
    :catch_0
    move-exception v0

    .line 313
    .local v0, "e":Ljava/lang/Exception;
    sget-object v2, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->TAG:Ljava/lang/String;

    invoke-static {v2, v0}, Lcom/linkedin/android/jobs/jobseeker/util/Utils;->safeToast(Ljava/lang/String;Ljava/lang/Object;)V

    goto :goto_0
.end method


# virtual methods
.method public getDisplayKey()Ljava/lang/String;
    .locals 1

    .prologue
    .line 342
    const-string v0, "view_settings_page"

    return-object v0
.end method

.method public getTracker()Lcom/linkedin/android/litrackinglib/metric/Tracker;
    .locals 2

    .prologue
    .line 337
    invoke-static {}, Lcom/linkedin/android/jobs/jobseeker/metrics/LiUnifiedTracking;->getInstance()Lcom/linkedin/android/jobs/jobseeker/metrics/LiUnifiedTracking;

    move-result-object v0

    const-string v1, "m_jobs"

    invoke-virtual {v0, v1, p0}, Lcom/linkedin/android/jobs/jobseeker/metrics/LiUnifiedTracking;->getTracker(Ljava/lang/String;Landroid/content/Context;)Lcom/linkedin/android/litrackinglib/metric/Tracker;

    move-result-object v0

    return-object v0
.end method

.method public handleLogoutClick(Landroid/view/View;)V
    .locals 4
    .param p1, "view"    # Landroid/view/View;

    .prologue
    .line 289
    invoke-static {}, Lcom/linkedin/android/jobs/jobseeker/listener/LogoutAlertDialogOnClickListener;->newInstance()Lcom/linkedin/android/jobs/jobseeker/listener/LogoutAlertDialogOnClickListener;

    move-result-object v0

    .line 291
    .local v0, "logoutAlertDialogOnClickListener":Lcom/linkedin/android/jobs/jobseeker/listener/LogoutAlertDialogOnClickListener;
    const v1, 0x7f0d00ea

    invoke-virtual {p0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getString(I)Ljava/lang/String;

    move-result-object v1

    const v2, 0x7f0d00eb

    invoke-virtual {p0, v2}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getString(I)Ljava/lang/String;

    move-result-object v2

    invoke-static {v1, v2, v0}, Lcom/linkedin/android/jobs/jobseeker/util/fragment/SimpleAlertDialogFragment;->newInstance(Ljava/lang/String;Ljava/lang/String;Lcom/linkedin/android/jobs/jobseeker/util/listener/ActivityAwareDialogOnClickListener;)Lcom/linkedin/android/jobs/jobseeker/util/fragment/SimpleAlertDialogFragment;

    move-result-object v1

    invoke-virtual {p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getFragmentManager()Landroid/app/FragmentManager;

    move-result-object v2

    const/4 v3, 0x0

    invoke-virtual {v1, v2, v3}, Lcom/linkedin/android/jobs/jobseeker/util/fragment/SimpleAlertDialogFragment;->show(Landroid/app/FragmentManager;Ljava/lang/String;)V

    .line 293
    return-void
.end method

.method public onCreate(Landroid/os/Bundle;)V
    .locals 34
    .param p1, "savedInstanceState"    # Landroid/os/Bundle;

    .prologue
    .line 67
    invoke-super/range {p0 .. p1}, Landroid/preference/PreferenceActivity;->onCreate(Landroid/os/Bundle;)V

    .line 73
    const v30, 0x7f03009d

    move-object/from16 v0, p0

    move/from16 v1, v30

    invoke-virtual {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->setContentView(I)V

    .line 74
    const v30, 0x7f0a01cf

    move-object/from16 v0, p0

    move/from16 v1, v30

    invoke-virtual {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->findViewById(I)Landroid/view/View;

    move-result-object v28

    check-cast v28, Landroid/support/v7/widget/Toolbar;

    .line 77
    .local v28, "toolbar":Landroid/support/v7/widget/Toolbar;
    const/16 v30, 0x1

    :try_start_0
    move-object/from16 v0, v28

    move/from16 v1, v30

    invoke-virtual {v0, v1}, Landroid/support/v7/widget/Toolbar;->setClickable(Z)V

    .line 78
    const v30, 0x7f0100a4

    move-object/from16 v0, p0

    move/from16 v1, v30

    invoke-static {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getResIdFromAttribute(Landroid/app/Activity;I)I

    move-result v30

    move-object/from16 v0, v28

    move/from16 v1, v30

    invoke-virtual {v0, v1}, Landroid/support/v7/widget/Toolbar;->setNavigationIcon(I)V

    .line 79
    const v30, 0x7f0d001d

    move-object/from16 v0, v28

    move/from16 v1, v30

    invoke-virtual {v0, v1}, Landroid/support/v7/widget/Toolbar;->setTitle(I)V

    .line 81
    new-instance v30, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$1;

    move-object/from16 v0, v30

    move-object/from16 v1, p0

    invoke-direct {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$1;-><init>(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;)V

    move-object/from16 v0, v28

    move-object/from16 v1, v30

    invoke-virtual {v0, v1}, Landroid/support/v7/widget/Toolbar;->setNavigationOnClickListener(Landroid/view/View$OnClickListener;)V
    :try_end_0
    .catch Ljava/lang/Exception; {:try_start_0 .. :try_end_0} :catch_1

    .line 91
    :goto_0
    const/high16 v30, 0x7f050000

    move-object/from16 v0, p0

    move/from16 v1, v30

    invoke-virtual {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->addPreferencesFromResource(I)V

    .line 95
    :try_start_1
    invoke-static {}, Lcom/linkedin/android/jobs/jobseeker/util/Utils;->getResources()Landroid/content/res/Resources;

    move-result-object v30

    const v31, 0x7f09006a

    invoke-virtual/range {v30 .. v31}, Landroid/content/res/Resources;->getDimension(I)F

    move-result v30

    move/from16 v0, v30

    float-to-int v0, v0

    move/from16 v18, v0

    .line 96
    .local v18, "paddingPixcel":I
    const v30, 0x102000a

    move-object/from16 v0, p0

    move/from16 v1, v30

    invoke-virtual {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->findViewById(I)Landroid/view/View;

    move-result-object v13

    .line 97
    .local v13, "listView":Landroid/view/View;
    const/16 v30, 0x0

    const/16 v31, 0x0

    move/from16 v0, v18

    move/from16 v1, v30

    move/from16 v2, v18

    move/from16 v3, v31

    invoke-virtual {v13, v0, v1, v2, v3}, Landroid/view/View;->setPadding(IIII)V

    .line 98
    invoke-virtual {v13}, Landroid/view/View;->getParent()Landroid/view/ViewParent;

    move-result-object v19

    check-cast v19, Landroid/view/ViewGroup;

    .line 99
    .local v19, "parent":Landroid/view/ViewGroup;
    const/16 v30, 0x0

    const/16 v31, 0x0

    const/16 v32, 0x0

    const/16 v33, 0x0

    move-object/from16 v0, v19

    move/from16 v1, v30

    move/from16 v2, v31

    move/from16 v3, v32

    move/from16 v4, v33

    invoke-virtual {v0, v1, v2, v3, v4}, Landroid/view/ViewGroup;->setPadding(IIII)V
    :try_end_1
    .catch Ljava/lang/Exception; {:try_start_1 .. :try_end_1} :catch_2

    .line 104
    .end local v13    # "listView":Landroid/view/View;
    .end local v18    # "paddingPixcel":I
    .end local v19    # "parent":Landroid/view/ViewGroup;
    :goto_1
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v30

    const-string v31, "privacy_inner"

    invoke-virtual/range {v30 .. v31}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v23

    .line 105
    .local v23, "privacyPreference":Landroid/preference/Preference;
    new-instance v30, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$2;

    move-object/from16 v0, v30

    move-object/from16 v1, p0

    invoke-direct {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$2;-><init>(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;)V

    move-object/from16 v0, v23

    move-object/from16 v1, v30

    invoke-virtual {v0, v1}, Landroid/preference/Preference;->setOnPreferenceClickListener(Landroid/preference/Preference$OnPreferenceClickListener;)V

    .line 115
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v30

    const-string v31, "help_center_pref"

    invoke-virtual/range {v30 .. v31}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v11

    .line 116
    .local v11, "helpCenterPreference":Landroid/preference/Preference;
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getApplicationContext()Landroid/content/Context;

    move-result-object v30

    invoke-static/range {v30 .. v30}, Lcom/linkedin/android/jobs/jobseeker/util/SessionUtils;->getHelpCenterUri(Landroid/content/Context;)Landroid/net/Uri;

    move-result-object v10

    .line 117
    .local v10, "helpCenterData":Landroid/net/Uri;
    sget-object v30, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->TAG:Ljava/lang/String;

    invoke-virtual {v10}, Landroid/net/Uri;->toString()Ljava/lang/String;

    move-result-object v31

    invoke-static/range {v30 .. v31}, Lcom/linkedin/android/jobs/jobseeker/util/Utils;->logString(Ljava/lang/String;Ljava/lang/String;)V

    .line 118
    invoke-virtual {v11}, Landroid/preference/Preference;->getIntent()Landroid/content/Intent;

    move-result-object v30

    move-object/from16 v0, v30

    invoke-virtual {v0, v10}, Landroid/content/Intent;->setData(Landroid/net/Uri;)Landroid/content/Intent;

    .line 120
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v30

    const-string v31, "pref_pn_your_application"

    invoke-virtual/range {v30 .. v31}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v20

    .line 121
    .local v20, "pnApplicationPref":Landroid/preference/Preference;
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v30

    const-string v31, "pref_pn_saved_searches"

    invoke-virtual/range {v30 .. v31}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v22

    .line 122
    .local v22, "pnSearchPref":Landroid/preference/Preference;
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v30

    const-string v31, "pref_pn_jobs_expiring"

    invoke-virtual/range {v30 .. v31}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v21

    .line 124
    .local v21, "pnJobsPref":Landroid/preference/Preference;
    invoke-static {}, Lcom/linkedin/android/jobs/jobseeker/listener/NotificationPreferenceChangeListener;->newInstance()Lcom/linkedin/android/jobs/jobseeker/listener/NotificationPreferenceChangeListener;

    move-result-object v30

    move-object/from16 v0, v20

    move-object/from16 v1, v30

    invoke-virtual {v0, v1}, Landroid/preference/Preference;->setOnPreferenceChangeListener(Landroid/preference/Preference$OnPreferenceChangeListener;)V

    .line 125
    invoke-static {}, Lcom/linkedin/android/jobs/jobseeker/listener/NotificationPreferenceChangeListener;->newInstance()Lcom/linkedin/android/jobs/jobseeker/listener/NotificationPreferenceChangeListener;

    move-result-object v30

    move-object/from16 v0, v22

    move-object/from16 v1, v30

    invoke-virtual {v0, v1}, Landroid/preference/Preference;->setOnPreferenceChangeListener(Landroid/preference/Preference$OnPreferenceChangeListener;)V

    .line 126
    invoke-static {}, Lcom/linkedin/android/jobs/jobseeker/listener/NotificationPreferenceChangeListener;->newInstance()Lcom/linkedin/android/jobs/jobseeker/listener/NotificationPreferenceChangeListener;

    move-result-object v30

    move-object/from16 v0, v21

    move-object/from16 v1, v30

    invoke-virtual {v0, v1}, Landroid/preference/Preference;->setOnPreferenceChangeListener(Landroid/preference/Preference$OnPreferenceChangeListener;)V

    .line 128
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v30

    const-string v31, "pref_shake_for_feedback"

    invoke-virtual/range {v30 .. v31}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v26

    .line 130
    .local v26, "shakeForFeedbackPref":Landroid/preference/Preference;
    new-instance v30, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$3;

    move-object/from16 v0, v30

    move-object/from16 v1, p0

    invoke-direct {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$3;-><init>(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;)V

    move-object/from16 v0, v26

    move-object/from16 v1, v30

    invoke-virtual {v0, v1}, Landroid/preference/Preference;->setOnPreferenceChangeListener(Landroid/preference/Preference$OnPreferenceChangeListener;)V

    .line 139
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v30

    const-string v31, "send_feedback"

    invoke-virtual/range {v30 .. v31}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v25

    .line 140
    .local v25, "sendFeedbackPref":Landroid/preference/Preference;
    new-instance v30, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$4;

    move-object/from16 v0, v30

    move-object/from16 v1, p0

    invoke-direct {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$4;-><init>(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;)V

    move-object/from16 v0, v25

    move-object/from16 v1, v30

    invoke-virtual {v0, v1}, Landroid/preference/Preference;->setOnPreferenceClickListener(Landroid/preference/Preference$OnPreferenceClickListener;)V

    .line 155
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v30

    const-string v31, "rate_the_app"

    invoke-virtual/range {v30 .. v31}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v24

    .line 156
    .local v24, "rateTheAppPref":Landroid/preference/Preference;
    new-instance v30, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$5;

    move-object/from16 v0, v30

    move-object/from16 v1, p0

    invoke-direct {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$5;-><init>(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;)V

    move-object/from16 v0, v24

    move-object/from16 v1, v30

    invoke-virtual {v0, v1}, Landroid/preference/Preference;->setOnPreferenceClickListener(Landroid/preference/Preference$OnPreferenceClickListener;)V

    .line 181
    :try_start_2
    invoke-static {}, Lcom/linkedin/android/jobs/jobseeker/util/Utils;->appIsDebugBuildOrUserIsLinkedInEmployee()Z

    move-result v30

    if-eqz v30, :cond_4

    .line 183
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v30

    const-string v31, "crash_app"

    invoke-virtual/range {v30 .. v31}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v6

    .line 184
    .local v6, "crashAppPref":Landroid/preference/Preference;
    new-instance v30, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$6;

    move-object/from16 v0, v30

    move-object/from16 v1, p0

    invoke-direct {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$6;-><init>(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;)V

    move-object/from16 v0, v30

    invoke-virtual {v6, v0}, Landroid/preference/Preference;->setOnPreferenceClickListener(Landroid/preference/Preference$OnPreferenceClickListener;)V

    .line 191
    invoke-static {}, Lcom/linkedin/android/jobs/jobseeker/util/Utils;->isDebugging()Z

    move-result v30

    if-eqz v30, :cond_0

    .line 193
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v30

    const-string v31, "simulate_invalid_token"

    invoke-virtual/range {v30 .. v31}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v27

    .line 195
    .local v27, "simulateInvalidTokenPref":Landroid/preference/Preference;
    new-instance v30, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$7;

    move-object/from16 v0, v30

    move-object/from16 v1, p0

    invoke-direct {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$7;-><init>(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;)V

    move-object/from16 v0, v27

    move-object/from16 v1, v30

    invoke-virtual {v0, v1}, Landroid/preference/Preference;->setOnPreferenceChangeListener(Landroid/preference/Preference$OnPreferenceChangeListener;)V

    .line 213
    .end local v27    # "simulateInvalidTokenPref":Landroid/preference/Preference;
    :cond_0
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v30

    const-string v31, "force_career_insights_on"

    invoke-virtual/range {v30 .. v31}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v9

    .line 215
    .local v9, "forceCareerInsightsOnPref":Landroid/preference/Preference;
    new-instance v30, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$8;

    move-object/from16 v0, v30

    move-object/from16 v1, p0

    invoke-direct {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$8;-><init>(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;)V

    move-object/from16 v0, v30

    invoke-virtual {v9, v0}, Landroid/preference/Preference;->setOnPreferenceClickListener(Landroid/preference/Preference$OnPreferenceClickListener;)V

    .line 231
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v30

    const-string v31, "pref_app_rater_debug"

    invoke-virtual/range {v30 .. v31}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v5

    .line 232
    .local v5, "appRaterDebugPref":Landroid/preference/Preference;
    new-instance v30, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$9;

    move-object/from16 v0, v30

    move-object/from16 v1, p0

    invoke-direct {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$9;-><init>(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;)V

    move-object/from16 v0, v30

    invoke-virtual {v5, v0}, Landroid/preference/Preference;->setOnPreferenceChangeListener(Landroid/preference/Preference$OnPreferenceChangeListener;)V

    .line 241
    const-string v30, "lix_member_header"

    move-object/from16 v0, p0

    move-object/from16 v1, v30

    invoke-virtual {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v16

    check-cast v16, Landroid/preference/PreferenceCategory;

    .line 242
    .local v16, "lixMemberPrefCategory":Landroid/preference/PreferenceCategory;
    sget-object v30, Lcom/linkedin/android/jobs/jobseeker/util/LixUtils;->MEMBER_BASED_LIX_KEYS:Ljava/util/Set;

    invoke-interface/range {v30 .. v30}, Ljava/util/Set;->iterator()Ljava/util/Iterator;

    move-result-object v12

    .local v12, "i$":Ljava/util/Iterator;
    :goto_2
    invoke-interface {v12}, Ljava/util/Iterator;->hasNext()Z

    move-result v30

    if-eqz v30, :cond_2

    invoke-interface {v12}, Ljava/util/Iterator;->next()Ljava/lang/Object;

    move-result-object v15

    check-cast v15, Ljava/lang/String;

    .line 243
    .local v15, "lixKey":Ljava/lang/String;
    new-instance v17, Lcom/linkedin/android/jobs/jobseeker/util/MultiLineSwitchPreference;

    move-object/from16 v0, v17

    move-object/from16 v1, p0

    invoke-direct {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/util/MultiLineSwitchPreference;-><init>(Landroid/content/Context;)V

    .line 244
    .local v17, "lixPref":Lcom/linkedin/android/jobs/jobseeker/util/MultiLineSwitchPreference;
    sget-object v30, Lcom/linkedin/android/jobs/jobseeker/util/LixUtils$LixType;->Member:Lcom/linkedin/android/jobs/jobseeker/util/LixUtils$LixType;

    move-object/from16 v0, v17

    move-object/from16 v1, v30

    invoke-static {v0, v1, v15}, Lcom/linkedin/android/jobs/jobseeker/fragment/SettingsFragment;->handleLixPref(Landroid/preference/SwitchPreference;Lcom/linkedin/android/jobs/jobseeker/util/LixUtils$LixType;Ljava/lang/String;)V

    .line 245
    invoke-virtual/range {v16 .. v17}, Landroid/preference/PreferenceCategory;->addPreference(Landroid/preference/Preference;)Z
    :try_end_2
    .catch Ljava/lang/Exception; {:try_start_2 .. :try_end_2} :catch_0

    goto :goto_2

    .line 267
    .end local v5    # "appRaterDebugPref":Landroid/preference/Preference;
    .end local v6    # "crashAppPref":Landroid/preference/Preference;
    .end local v9    # "forceCareerInsightsOnPref":Landroid/preference/Preference;
    .end local v12    # "i$":Ljava/util/Iterator;
    .end local v15    # "lixKey":Ljava/lang/String;
    .end local v16    # "lixMemberPrefCategory":Landroid/preference/PreferenceCategory;
    .end local v17    # "lixPref":Lcom/linkedin/android/jobs/jobseeker/util/MultiLineSwitchPreference;
    :catch_0
    move-exception v7

    .line 268
    .local v7, "e":Ljava/lang/Exception;
    invoke-direct/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->removeInternalSection()V

    .line 271
    .end local v7    # "e":Ljava/lang/Exception;
    :goto_3
    invoke-static/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/presenter/LogOutEventPresenter;->newInstance(Landroid/app/Activity;)Lcom/linkedin/android/jobs/jobseeker/presenter/LogOutEventPresenter;

    move-result-object v30

    move-object/from16 v0, v30

    move-object/from16 v1, p0

    iput-object v0, v1, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->_presenter:Lcom/linkedin/android/jobs/jobseeker/presenter/LogOutEventPresenter;

    .line 272
    move-object/from16 v0, p0

    iget-object v0, v0, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->_presenter:Lcom/linkedin/android/jobs/jobseeker/presenter/LogOutEventPresenter;

    move-object/from16 v30, v0

    invoke-static/range {v30 .. v30}, Lcom/linkedin/android/jobs/jobseeker/subject/LogOutEventSubject;->subscribe(Lrx/Observer;)Lrx/Subscription;

    move-result-object v30

    move-object/from16 v0, v30

    move-object/from16 v1, p0

    iput-object v0, v1, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->_subscription:Lrx/Subscription;

    .line 275
    invoke-static {}, Lcom/linkedin/android/jobs/jobseeker/util/Utils;->isDebugging()Z

    move-result v30

    if-nez v30, :cond_1

    .line 276
    invoke-static/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/util/Utils;->lockScreenToPortraitOrientation(Landroid/app/Activity;)V

    .line 278
    :cond_1
    return-void

    .line 87
    .end local v10    # "helpCenterData":Landroid/net/Uri;
    .end local v11    # "helpCenterPreference":Landroid/preference/Preference;
    .end local v20    # "pnApplicationPref":Landroid/preference/Preference;
    .end local v21    # "pnJobsPref":Landroid/preference/Preference;
    .end local v22    # "pnSearchPref":Landroid/preference/Preference;
    .end local v23    # "privacyPreference":Landroid/preference/Preference;
    .end local v24    # "rateTheAppPref":Landroid/preference/Preference;
    .end local v25    # "sendFeedbackPref":Landroid/preference/Preference;
    .end local v26    # "shakeForFeedbackPref":Landroid/preference/Preference;
    :catch_1
    move-exception v7

    .line 88
    .restart local v7    # "e":Ljava/lang/Exception;
    const/16 v30, 0x0

    move-object/from16 v0, v28

    move/from16 v1, v30

    invoke-static {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/util/Utils;->showOrGoneView(Landroid/view/View;Z)V

    goto/16 :goto_0

    .line 100
    .end local v7    # "e":Ljava/lang/Exception;
    :catch_2
    move-exception v8

    .line 101
    .local v8, "ex":Ljava/lang/Exception;
    sget-object v30, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->TAG:Ljava/lang/String;

    move-object/from16 v0, v30

    invoke-static {v0, v8}, Lcom/linkedin/android/jobs/jobseeker/util/Utils;->safeToast(Ljava/lang/String;Ljava/lang/Object;)V

    goto/16 :goto_1

    .line 248
    .end local v8    # "ex":Ljava/lang/Exception;
    .restart local v5    # "appRaterDebugPref":Landroid/preference/Preference;
    .restart local v6    # "crashAppPref":Landroid/preference/Preference;
    .restart local v9    # "forceCareerInsightsOnPref":Landroid/preference/Preference;
    .restart local v10    # "helpCenterData":Landroid/net/Uri;
    .restart local v11    # "helpCenterPreference":Landroid/preference/Preference;
    .restart local v12    # "i$":Ljava/util/Iterator;
    .restart local v16    # "lixMemberPrefCategory":Landroid/preference/PreferenceCategory;
    .restart local v20    # "pnApplicationPref":Landroid/preference/Preference;
    .restart local v21    # "pnJobsPref":Landroid/preference/Preference;
    .restart local v22    # "pnSearchPref":Landroid/preference/Preference;
    .restart local v23    # "privacyPreference":Landroid/preference/Preference;
    .restart local v24    # "rateTheAppPref":Landroid/preference/Preference;
    .restart local v25    # "sendFeedbackPref":Landroid/preference/Preference;
    .restart local v26    # "shakeForFeedbackPref":Landroid/preference/Preference;
    :cond_2
    :try_start_3
    const-string v30, "lix_device_header"

    move-object/from16 v0, p0

    move-object/from16 v1, v30

    invoke-virtual {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v14

    check-cast v14, Landroid/preference/PreferenceCategory;

    .line 249
    .local v14, "lixDevicePrefCategory":Landroid/preference/PreferenceCategory;
    sget-object v30, Lcom/linkedin/android/jobs/jobseeker/util/LixUtils;->DEVICE_BASED_LIX_KEYS:Ljava/util/Set;

    invoke-interface/range {v30 .. v30}, Ljava/util/Set;->iterator()Ljava/util/Iterator;

    move-result-object v12

    :goto_4
    invoke-interface {v12}, Ljava/util/Iterator;->hasNext()Z

    move-result v30

    if-eqz v30, :cond_3

    invoke-interface {v12}, Ljava/util/Iterator;->next()Ljava/lang/Object;

    move-result-object v15

    check-cast v15, Ljava/lang/String;

    .line 250
    .restart local v15    # "lixKey":Ljava/lang/String;
    new-instance v17, Lcom/linkedin/android/jobs/jobseeker/util/MultiLineSwitchPreference;

    move-object/from16 v0, v17

    move-object/from16 v1, p0

    invoke-direct {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/util/MultiLineSwitchPreference;-><init>(Landroid/content/Context;)V

    .line 251
    .restart local v17    # "lixPref":Lcom/linkedin/android/jobs/jobseeker/util/MultiLineSwitchPreference;
    sget-object v30, Lcom/linkedin/android/jobs/jobseeker/util/LixUtils$LixType;->Device:Lcom/linkedin/android/jobs/jobseeker/util/LixUtils$LixType;

    move-object/from16 v0, v17

    move-object/from16 v1, v30

    invoke-static {v0, v1, v15}, Lcom/linkedin/android/jobs/jobseeker/fragment/SettingsFragment;->handleLixPref(Landroid/preference/SwitchPreference;Lcom/linkedin/android/jobs/jobseeker/util/LixUtils$LixType;Ljava/lang/String;)V

    .line 252
    move-object/from16 v0, v17

    invoke-virtual {v14, v0}, Landroid/preference/PreferenceCategory;->addPreference(Landroid/preference/Preference;)Z

    goto :goto_4

    .line 256
    .end local v15    # "lixKey":Ljava/lang/String;
    .end local v17    # "lixPref":Lcom/linkedin/android/jobs/jobseeker/util/MultiLineSwitchPreference;
    :cond_3
    invoke-virtual/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;

    move-result-object v30

    const-string v31, "unified_metrics"

    invoke-virtual/range {v30 .. v31}, Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v29

    .line 257
    .local v29, "unifiedMetricPref":Landroid/preference/Preference;
    new-instance v30, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$10;

    move-object/from16 v0, v30

    move-object/from16 v1, p0

    invoke-direct {v0, v1}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$10;-><init>(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;)V

    invoke-virtual/range {v29 .. v30}, Landroid/preference/Preference;->setOnPreferenceChangeListener(Landroid/preference/Preference$OnPreferenceChangeListener;)V

    goto/16 :goto_3

    .line 265
    .end local v5    # "appRaterDebugPref":Landroid/preference/Preference;
    .end local v6    # "crashAppPref":Landroid/preference/Preference;
    .end local v9    # "forceCareerInsightsOnPref":Landroid/preference/Preference;
    .end local v12    # "i$":Ljava/util/Iterator;
    .end local v14    # "lixDevicePrefCategory":Landroid/preference/PreferenceCategory;
    .end local v16    # "lixMemberPrefCategory":Landroid/preference/PreferenceCategory;
    .end local v29    # "unifiedMetricPref":Landroid/preference/Preference;
    :cond_4
    invoke-direct/range {p0 .. p0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->removeInternalSection()V
    :try_end_3
    .catch Ljava/lang/Exception; {:try_start_3 .. :try_end_3} :catch_0

    goto/16 :goto_3
.end method

.method protected onDestroy()V
    .locals 2

    .prologue
    const/4 v1, 0x0

    .line 282
    invoke-super {p0}, Landroid/preference/PreferenceActivity;->onDestroy()V

    .line 283
    iget-object v0, p0, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->_subscription:Lrx/Subscription;

    invoke-static {v0}, Lcom/linkedin/android/jobs/jobseeker/subject/LogOutEventSubject;->unSubscribe(Lrx/Subscription;)V

    .line 284
    iput-object v1, p0, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->_presenter:Lcom/linkedin/android/jobs/jobseeker/presenter/LogOutEventPresenter;

    .line 285
    iput-object v1, p0, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->_subscription:Lrx/Subscription;

    .line 286
    return-void
.end method

.method public onOptionsItemSelected(Landroid/view/MenuItem;)Z
    .locals 1
    .param p1, "item"    # Landroid/view/MenuItem;

    .prologue
    .line 327
    invoke-interface {p1}, Landroid/view/MenuItem;->getItemId()I

    move-result v0

    packed-switch v0, :pswitch_data_0

    .line 332
    invoke-super {p0, p1}, Landroid/preference/PreferenceActivity;->onOptionsItemSelected(Landroid/view/MenuItem;)Z

    move-result v0

    :goto_0
    return v0

    .line 329
    :pswitch_0
    invoke-static {p0}, Landroid/support/v4/app/NavUtils;->navigateUpFromSameTask(Landroid/app/Activity;)V

    .line 330
    const/4 v0, 0x1

    goto :goto_0

    .line 327
    nop

    :pswitch_data_0
    .packed-switch 0x102002c
        :pswitch_0
    .end packed-switch
.end method

.method protected onPostCreate(Landroid/os/Bundle;)V
    .locals 2
    .param p1, "savedInstanceState"    # Landroid/os/Bundle;

    .prologue
    .line 302
    invoke-super {p0, p1}, Landroid/preference/PreferenceActivity;->onPostCreate(Landroid/os/Bundle;)V

    .line 303
    sget-object v0, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$SettingKeys;->VERSION:Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$SettingKeys;

    invoke-direct {p0, v0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPrefName(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$SettingKeys;)Ljava/lang/String;

    move-result-object v0

    invoke-virtual {p0, v0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v0

    invoke-static {p0}, Lcom/linkedin/android/jobs/jobseeker/util/ApplicationUtils;->getApplicationVersion(Landroid/content/Context;)Ljava/lang/String;

    move-result-object v1

    invoke-virtual {v0, v1}, Landroid/preference/Preference;->setSummary(Ljava/lang/CharSequence;)V

    .line 304
    sget-object v0, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$SettingKeys;->BUILD:Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$SettingKeys;

    invoke-direct {p0, v0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPrefName(Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity$SettingKeys;)Ljava/lang/String;

    move-result-object v0

    invoke-virtual {p0, v0}, Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;

    move-result-object v0

    invoke-static {}, Lcom/linkedin/android/jobs/jobseeker/util/ApplicationUtils;->getBuildVersion()Ljava/lang/String;

    move-result-object v1

    invoke-virtual {v0, v1}, Landroid/preference/Preference;->setSummary(Ljava/lang/CharSequence;)V

    .line 305
    return-void
.end method

.method protected onResume()V
    .locals 0

    .prologue
    .line 347
    invoke-super {p0}, Landroid/preference/PreferenceActivity;->onResume()V

    .line 349
    invoke-static {p0}, Lcom/linkedin/android/jobs/jobseeker/util/MetricUtils;->sendDisplayMetric(Lcom/linkedin/android/litrackinglib/pages/IDisplayKeyProvider;)V

    .line 350
    return-void
.end method

using LinkedMovement.Animation;
using LinkedMovement.Links;
using LinkedMovement.UI;
using LinkedMovement.UI.Utils;
using LinkedMovement.Utils;
using Parkitect.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LinkedMovement {
    public class LMController : MonoBehaviour {

        // TODO: 11-10
        //
        // Check # Tweens being created matches expected

        // Triggers
        //
        // non fatal exception
        // Appears to happen when selecting a triggered animation in the Effects Controller
        // This happens with OLD system as well. Possibly ignore.
        // NullReferenceException: Object reference not set to an instance of an object
        //   at AnimationTriggerEffectEditorPanel.initialize()[0x00011] in <eefe76887ca042e485a07fadc6c705a6>:0 

        // Once saw issue with animations, when at end of sequence, having 1 frame of child object mispositioned
        // Haven't seen since first occurrence.

        public WindowManager windowManager;

        public LMAnimation currentAnimation { get; private set; }
        public LMLink currentLink { get; private set; }
        
        private List<LMAnimation> animations = new List<LMAnimation>();
        private List<LMLink> links = new List<LMLink>();

        private HashSet<BuildableObject> buildableObjectsToUpdate = new HashSet<BuildableObject>();

        private bool mouseToolActive = false;
        private HashSet<BuildableObject> animationHelperObjects = new HashSet<BuildableObject>();

        private LMAnimation queuedAnimationToRemove;
        private LMLink queuedLinkToRemove;

        // For lack of better place for this
        private ColorPickerWindow colorPickerWindow;

        private Dictionary<string, List<LMAnimationStep>> savedAnimationSteps = new Dictionary<string, List<LMAnimationStep>>();
        
        private void Awake() {
            LMLogger.Debug("LMController Awake");

            windowManager = new WindowManager();
        }

        private void OnDisable() {
            LMLogger.Debug("LMController OnDisable");
        }

        private void OnDestroy() {
            LMLogger.Debug("LMController OnDestroy");

            if (windowManager != null) {
                windowManager.destroy();
                windowManager = null;
            }

            foreach (var animation in animations) {
                animation.stopSequenceImmediate();
            }
            animations.Clear();
            links.Clear();
            animationHelperObjects.Clear();
            LinkedMovement.ClearLMController();
        }

        private void Update() {
            if (queuedAnimationToRemove != null) {
                doRemoveAnimation(queuedAnimationToRemove);
            }
            if (queuedLinkToRemove != null) {
                doRemoveLink(queuedLinkToRemove);
            }

            if (UIUtility.isInputFieldFocused() || GameController.Instance.isGameInputLocked() || GameController.Instance.isQuittingGame) {
                return;
            }

            if (InputManager.getKeyUp("LM_toggleGUI") && !windowManager.uiPresent()) {
                LMLogger.Debug("Toggle GUI");
                windowManager.createWindow(WindowManager.WindowType.ModeDeterminationNew, null);
            }

            // If there is no mouse tool active & not in builder mode (Deco, Blueprints), we don't need to update mouse colliders
            var mouseTool = GameController.Instance.getActiveMouseTool();
            if (mouseTool == null && !GameController.Instance.hasActiveBuilderWindow()) {
                return;
            }

            buildableObjectsToUpdate.Clear();
            foreach (var link in links) {
                link.addObjectsToUpdateMouseColliders(buildableObjectsToUpdate);
            }
            foreach (var animation in animations) {
                buildableObjectsToUpdate.Add(animation.targetBuildableObject);
            }
            foreach (var bo in buildableObjectsToUpdate) {
                LMUtils.UpdateMouseColliders(bo);
            }
        }

        private void OnGUI() {
            if (OptionsMenu.instance != null) return;

            float uiScale = Settings.Instance.uiScale;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(uiScale, uiScale, 1f));
            windowManager.DoGUI();
            GUI.matrix = Matrix4x4.identity;
        }

        public List<LMAnimation> getAnimations() {
            return animations;
        }

        public int getNumAnimations() {
            return animations.Count;
        }

        public List<LMLink> getLinks() {
            return links;
        }

        public void setMouseToolActive(bool active) {
            mouseToolActive = active;
            updateAnimationHelperVisibility();
        }

        public void addAnimationHelper(BuildableObject buildableObject) {
            LMLogger.Debug("Add animationHelper " + buildableObject.name);
            animationHelperObjects.Add(buildableObject);
            var renderer = buildableObject.gameObject.GetComponent<Renderer>();
            if (renderer != null) {
                renderer.enabled = shouldShowAnimationHelperObjects();
            }
        }

        public void removeAnimationHelper(BuildableObject buildableObject) {
            LMLogger.Debug("Remove animationHelper " + buildableObject.name);
            animationHelperObjects.Remove(buildableObject);
        }

        private bool shouldShowAnimationHelperObjects() {
            return mouseToolActive || currentAnimation != null || currentLink != null;
        }

        private void updateAnimationHelperVisibility() {
            var show = shouldShowAnimationHelperObjects();

            foreach (var animationHelperObject in animationHelperObjects) {
                var renderer = animationHelperObject.gameObject.GetComponent<Renderer>();
                if (renderer != null) {
                    renderer.enabled = show;
                }
            }
        }

        // Called via ParkEventStartPostFix
        public void setupPark(List<SerializedMonoBehaviour> serializedMonoBehaviours) {
            LMLogger.Debug($"LMController.setPark with {serializedMonoBehaviours.Count} objects");
            
            var createdLinkParents = new List<LMLinkParent>();
            var createdLinkTargets = new List<LMLinkTarget>();

            // TODO: Does this need to be reversed?
            for (int i = serializedMonoBehaviours.Count - 1; i >= 0; i--) {
                var smb = serializedMonoBehaviours[i];

                LMLinkParent linkParent = LMUtils.GetLinkParentFromSerializedMonoBehaviour(smb);
                if (linkParent != null) {
                    LMLogger.Debug($"Found LinkParent name: {linkParent.name}, id: {linkParent.id}");
                    linkParent.setTarget(smb.gameObject);
                    createdLinkParents.Add(linkParent);
                    LMUtils.DeleteChunkedMesh(smb as BuildableObject);
                }

                LMLinkTarget linkTarget = LMUtils.GetLinkTargetFromSerializedMonoBehaviour(smb);
                if (linkTarget != null) {
                    LMLogger.Debug($"Found LinkTarget id: {linkTarget.id}");
                    linkTarget.setTarget(smb.gameObject);
                    createdLinkTargets.Add(linkTarget);
                    LMUtils.DeleteChunkedMesh(smb as BuildableObject);
                }

                LMAnimationParams animationParams = LMUtils.GetAnimationParamsFromSerializedMonoBehaviour(smb);
                if (animationParams != null) {
                    LMLogger.Debug($"Found AnimationParams name: {animationParams.name}, id: {animationParams.id}");
                    addAnimation(animationParams, smb.gameObject);
                    LMUtils.DeleteChunkedMesh(smb as BuildableObject);
                }
            }

            // TODO: Do we need to find orphaned PairTargets?

            setupLinks(createdLinkParents, createdLinkTargets);
            onParkStarted();
        }

        public void setupLinks(List<LMLinkParent> linkParents, List<LMLinkTarget> linkTargets) {
            LMLogger.Debug($"LMController.setupLinks from {linkParents.Count} LinkParents and {linkTargets.Count} LinkTargets");

            foreach (var linkParent in linkParents) {
                LMLogger.Debug($"Find targets for {linkParent.name}, id {linkParent.id}");
                var matchingTargets = getLinkTargetsById(linkParent.id, linkTargets);
                LMLogger.Debug($"Found {matchingTargets.Count} matching targets");
                if (matchingTargets.Count > 0) {
                    var link = new LMLink(linkParent, matchingTargets);
                    LMLogger.Debug("Adding and building Link " + link.name);
                    links.Add(link);
                    link.rebuildLink();
                }
            }
        }

        public void setupLinks(List<LMLink> newLinks) {
            LMLogger.Debug("LMController.setupLinks from Links list, count: " + newLinks.Count);

            foreach (var link in newLinks) {
                LMLogger.Debug($"Adding and building Link name: {link.name}, id: {link.id}");
                links.Add(link);
                link.rebuildLink();
            }
        }

        public void setupAnimations(List<LMAnimation> newAnimations) {
            LMLogger.Debug("LMController.setupAnimations from Animations list, count: " + newAnimations.Count);

            foreach (var anim in newAnimations) {
                LMLogger.Debug($"Adding and building Animation name: {anim.name}, id: {anim.id}");
                animations.Add(anim);
                anim.setup();
            }
        }

        public void onParkStarted() {
            LMLogger.Debug($"LMController.onParkStarted with {animations.Count} animations");

            foreach (var animation in animations) {
                animation.setup();
            }
        }

        public void handleBuildableObjectDestruct(BuildableObject buildableObject) {
            LMLogger.Debug("LMController.handleBuildableObjectDestruct");
            if (buildableObject == null || buildableObject.gameObject == null) {
                LMLogger.Debug("Missing object (BO or GO)");
                return;
            }
            if (buildableObject.isPreview) {
                LMLogger.Debug("Object is preview, skip");
                return;
            }

            var gameObject = buildableObject.gameObject;
            LMLogger.Debug("Destruct object: " + gameObject.name);

            var animation = findAnimationByGameObject(gameObject);
            var linkAsParent = findLinkByParentGameObject(gameObject);
            var linkAsTarget = findLinkByTargetGameObject(gameObject);

            // If LinkParent, LinkTarget, or Animation target
            // - stop associated (this will put all associated at starting values)
            if (animation != null || linkAsParent != null || linkAsTarget != null) {
                LMLogger.Debug("Deleted object is associated with an Animation or Links");
                LMUtils.EditAssociatedAnimations(new List<GameObject>() { gameObject }, LMUtils.AssociatedAnimationEditMode.Stop, false);
            } else {
                LMLogger.Debug("Deleted object not associated with any Animation or Links, exit");
                return;
            }

            // Create restart list
            var restartList = new HashSet<GameObject>();

            // If Animation
            // - remove from animations
            // - delete Animation (remove data)
            if (animation != null) {
                LMLogger.Debug("deleted object has animation " + animation.name);
                animations.Remove(animation);
                animation.destroyAnimation();
            }

            // If LinkParent
            // - remove from links
            // - add children to restart list
            // - delete link (unparent children, remove data)
            if (linkAsParent != null) {
                LMLogger.Debug($"deleted object is parent for Link name: {linkAsParent.name}, id: {linkAsParent.id}");
                links.Remove(linkAsParent);
                var targetGameObjects = linkAsParent.getTargetGameObjects();
                foreach (var targetGameObject in targetGameObjects) {
                    restartList.Add(targetGameObject);
                }
                linkAsParent.destroyLink();
            }

            // If LinkTarget
            // - add LinkParent to restart list
            // - unparent from LinkParent
            // - if LinkParent has no children
            // -- remove from links
            // - delete data
            if (linkAsTarget != null) {
                LMLogger.Debug($"deleted object is target for Link name: {linkAsTarget.name}, id: {linkAsTarget.id}");
                restartList.Add(linkAsTarget.getParentGameObject());
                linkAsTarget.deleteTargetObject(gameObject);

                if (!linkAsTarget.isValid()) {
                    LMLogger.Debug("Link no longer valid, remove");
                    links.Remove(linkAsTarget);
                    linkAsTarget.destroyLink();
                } else {
                    // Link is still valid, restart parent
                    restartList.Add(linkAsTarget.getParentGameObject());
                }
            }

            // Restart list
            if (restartList.Count > 0) {
                LMLogger.Debug($"Try to restart animations on {restartList.Count} objects");
                var restartListGameObjects = new List<GameObject>();
                foreach (var hashObject in restartList) {
                    restartListGameObjects.Add(hashObject);
                }
                LMUtils.EditAssociatedAnimations(restartListGameObjects, LMUtils.AssociatedAnimationEditMode.Start, false);
            }
        }

        public void clearEditMode() {
            LMLogger.Debug("LMController.clearEditMode");
            closeColorPickerWindow();
            windowManager.closeAllWindows();

            if (currentAnimation != null) {
                currentAnimation.discardChanges();
                currentAnimation = null;
            }
            if (currentLink != null) {
                currentLink.discardChanges();
                currentLink = null;
            }

            updateAnimationHelperVisibility();
        }

        public LMAnimation findAnimationByGameObject(GameObject gameObject) {
            LMLogger.Debug("LMController.findAnimationByGameObject");
            foreach (var animation in animations) {
                if (animation.targetGameObject == gameObject) {
                    return animation;
                }
            }
            LMLogger.Debug("No animation found");
            return null;
        }

        public LMAnimation addAnimation(LMAnimationParams animationParams, GameObject target) {
            LMLogger.Debug("LMController.addAnimation from LMAnimationParams");
            
            var animation = new LMAnimation(animationParams, target, true);
            addAnimation(animation);
            return animation;
        }

        public void addAnimation(LMAnimation animation) {
            LMLogger.Debug("LMController.addAnimation from LMAnimation");

            if (animations.Contains(animation)) {
                LMLogger.Debug("Animation already in controller list");
                return;
            }

            animations.Add(animation);
        }

        public void addLink(LMLink link) {
            LMLogger.Debug("LMController.addLink from LMLink");

            if (links.Contains(link)) {
                LMLogger.Debug("Link already in controller list");
                return;
            }

            links.Add(link);
        }

        public void editAnimation(LMAnimation animation = null) {
            LMLogger.Debug("LMController.editAnimation");
            clearEditMode();

            if (animation != null) {
                LMLogger.Debug("Edit existing Animation");
                currentAnimation = animation;
            } else {
                // TODO: Set new animation name
                LMLogger.Debug("Edit new Animation");
                var animationParams = new LMAnimationParams();
                currentAnimation = new LMAnimation(animationParams);
            }

            currentAnimation.IsEditing = true;

            if (animation != null) {
                currentAnimation.buildSequence();
            }

            updateAnimationHelperVisibility();
        }

        public void editLink(LMLink link = null) {
            LMLogger.Debug("LMController.editLink");
            clearEditMode();

            if (link != null) {
                LMLogger.Debug("Edit existing link");
                currentLink = link;
            } else {
                LMLogger.Debug("Create new link");
                // TODO: set new link name
                currentLink = new LMLink();
            }

            currentLink.IsEditing = true;

            updateAnimationHelperVisibility();
        }

        public void queueAnimationToRemove(LMAnimation animation) {
            LMLogger.Debug($"LMController.queueAnimationToRemove name: {animation.name}, id: {animation.id}");
            queuedAnimationToRemove = animation;
        }

        private void doRemoveAnimation(LMAnimation animation) {
            LMLogger.Debug($"LMController.doRemoveAnimation name: {animation.name}, id: {animation.id}");

            var animationGameObject = animation.targetGameObject;
            var goList = new List<GameObject>() { animationGameObject };
            LMUtils.EditAssociatedAnimations(goList, LMUtils.AssociatedAnimationEditMode.Stop, false);

            animations.Remove(animation);
            animation.destroyAnimation();

            LMUtils.EditAssociatedAnimations(goList, LMUtils.AssociatedAnimationEditMode.Start, false);
            queuedAnimationToRemove = null;
        }

        public void queueLinkToRemove(LMLink link) {
            LMLogger.Debug($"LMController.queueLinkToRemove name: {link.name}, id: {link.id}");
            queuedLinkToRemove = link;
        }

        private void doRemoveLink(LMLink link) {
            LMLogger.Debug($"LMController.doRemoveList name: {link.name}, id: {link.id}");

            var linkParentGameObject = link.getParentGameObject();
            var linkTargetGameObjects = link.getTargetGameObjects();
            var parentGameObjectList = new List<GameObject>() { linkParentGameObject };
            var allGameObjectList = new List<GameObject>() { linkParentGameObject };
            allGameObjectList.AddRange(linkTargetGameObjects);

            LMUtils.EditAssociatedAnimations(parentGameObjectList, LMUtils.AssociatedAnimationEditMode.Stop, false);

            links.Remove(link);
            link.destroyLink();

            LMUtils.EditAssociatedAnimations(allGameObjectList, LMUtils.AssociatedAnimationEditMode.Start, false);

            queuedLinkToRemove = null;
        }

        public void commitEdit() {
            LMLogger.Debug("LMController.commitEdit");
            
            if (currentAnimation != null) {
                LMUtils.EditAssociatedAnimations(new List<GameObject>() { currentAnimation.targetGameObject }, LMUtils.AssociatedAnimationEditMode.Restart, true);
                currentAnimation.saveChanges();
                currentAnimation = null;
            }
            if (currentLink != null) {
                currentLink.saveChanges();
                currentLink = null;
            }

            clearEditMode();
        }

        public void currentAnimationUpdated(bool animationLengthWasChanged = false) {
            LMLogger.Debug("LMController.currentAnimationUpdated");
            // TODO: Should this be an event handler subscribed to LMAnimation?
            // + Eliminates direct calls to controller
            // - Muddies control flow

            // Animation was updated, rebuild
            if (animationLengthWasChanged) {
                currentAnimation.getAnimationParams().animationLengthWasChanged();
            }
            LMUtils.EditAssociatedAnimations(new List<GameObject>() { currentAnimation.targetGameObject }, LMUtils.AssociatedAnimationEditMode.Restart, true);

            currentAnimation.buildSequence();
        }

        public LMLink findLinkByParentGameObject(GameObject gameObject) {
            LMLogger.Debug("LMController.findLinkByParentGameObject");
            foreach (var link in links) {
                if (link.getParentGameObject() == gameObject) {
                    LMLogger.Debug("Found Link from parent");
                    return link;
                }
            }
            LMLogger.Debug("No Link found from parent");
            return null;
        }

        public LMLink findLinkByTargetGameObject(GameObject gameObject) {
            LMLogger.Debug("LMController.findLinkByTargetGameObject");
            foreach (var link in links) {
                var targets = link.getTargetGameObjects();
                if (targets.Contains(gameObject)) {
                    LMLogger.Debug("Found Link from target");
                    return link;
                }
            }
            LMLogger.Debug("No Link found from target");
            return null;
        }

        public bool colorPickerWindowIsOpen() {
            return colorPickerWindow != null;
        }

        public ColorPickerWindow getColorPickerWindow() {
            return colorPickerWindow;
        }

        public void closeColorPickerWindow() {
            if (colorPickerWindow != null) {
                ColorPickerWindow.closeOpenInstance();
                colorPickerWindow = null;
            }
        }

        public ColorPickerWindow launchColorPickerWindow(Color[] colors, int selectedIndex) {
            colorPickerWindow = ColorPickerWindow.launch(colors, selectedIndex);
            colorPickerWindow.onClose += () => {
                colorPickerWindow = null;
            };
            return colorPickerWindow;
        }

        public void restartAllAnimations() {
            HashSet<GameObject> animatedGameObjectsHashSet = new HashSet<GameObject>();
            foreach (var animation in animations) {
                animatedGameObjectsHashSet.Add(animation.targetGameObject);
            }
            List<GameObject> animatedGameObjectsList = animatedGameObjectsHashSet.ToList<GameObject>();
            LMUtils.EditAssociatedAnimations(animatedGameObjectsList, LMUtils.AssociatedAnimationEditMode.Stop, false);
            LMUtils.EditAssociatedAnimations(animatedGameObjectsList, LMUtils.AssociatedAnimationEditMode.Start, false);
        }

        public Dictionary<string, List<LMAnimationStep>> getSavedAnimationSteps() {
            return savedAnimationSteps;
        }

        public void addSavedAnimationSteps(string name, List<LMAnimationStep> steps) {
            LMLogger.Debug("LMController.addSavedAnimationSteps name: " + name);
            var exists = savedAnimationSteps.ContainsKey(name);
            if (exists) {
                string infoMessage = LMStringSystem.GetText(LMStringKey.SAVED_ANIMATION_ALREADY_EXISTS, name);
                windowManager.createWindow(UI.WindowManager.WindowType.Information, infoMessage);
                savedAnimationSteps[name] = steps;
            } else {
                savedAnimationSteps.Add(name, steps);
            }
        }

        private List<LMLinkTarget> getLinkTargetsById(string id, List<LMLinkTarget> targets) {
            LMLogger.Debug("LMController.getLinkTargetsById: " + id);
            var matchingTargets = new List<LMLinkTarget>();

            foreach (var target in targets) {
                if (target.id == id) matchingTargets.Add(target);
            }

            LMLogger.Debug($"Got {matchingTargets.Count} targets");
            return matchingTargets;
        }

    }
}
